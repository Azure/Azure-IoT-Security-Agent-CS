// <copyright file="AuditEventGeneratorBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// A base class for generators that rely on linux audit events (auditd)
    /// </summary>
    public abstract class AuditEventGeneratorBase : EventGenerator
    {
        private const string AusearchNoMatchesText = "<no matches>\n";
        private const string AuditctlRuleAlreadyDefinedText = "Error sending add rule data request (Rule exists)\n";
        private const int AusearchInvalidCheckpointData = 10;
        private const int AusearchCheckpointProcessingError = 11;
        private const int AusearchCheckpointEventNotFoundInLog = 12;

        private string UnameMachine = null;

        private readonly IProcessUtil _processUtil;

        /// <summary>
        /// A list of auditctl rules that must be configured in order for this generator to work
        /// </summary>
        protected abstract IEnumerable<string> AuditRulesPrerequisites { get; }

        /// <summary>
        /// The audit event types this generator relies on
        /// </summary>
        protected abstract IEnumerable<AuditEventType> AuditEventTypes { get; }

        /// <summary>
        /// The arguments used for filtering the audit records, by default this is constructed from the AuditEventTypes
        /// </summary>
        protected virtual string RecordFilteringArguments
        {
            get
            {
                string arguments = string.Join(',', AuditEventTypes.Select(item => item.GetAttribute<DisplayAttribute>().Name));
                return string.Join(' ', "-m", arguments);
            }
        }

        /// <summary>
        /// Constructor for AuditEvent
        /// The constructor runs the rule prerequisites
        /// </summary>
        protected AuditEventGeneratorBase(IProcessUtil processUtil)
        {
            _processUtil = processUtil;

            string unameMachine = this.GetUnameMachine();

            foreach (string rule in AuditRulesPrerequisites)
            {
                string formattedRule = string.Format(rule, unameMachine);
                string command = $"sudo auditctl {formattedRule}";

                try
                {
                    _processUtil.ExecuteBashShellCommand(command);
                }
                catch (CommandExecutionFailedException ex)
                {
                    if (ex.Error == AuditctlRuleAlreadyDefinedText)
                    {
                        SimpleLogger.Debug("rule already defined: " + command);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            //establish checkpoint
            ExecuteAusearchWithFallback();
        }

        /// <inheritdoc/>
        public override IEnumerable<IEvent> GetEvents()
        {
            string searchResultsString = ExecuteAusearchWithFallback();
            IEnumerable<string> searchResults = searchResultsString.Split("----", StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<AuditEvent> auditEvent = searchResults.Select(AuditEvent.ParseFromAusearchLine);

            return GetEventsImpl(auditEvent).ToList();
        }

        /// <summary>
        /// The actual implementation of GetEvents for the particular generator, converts audit events to security events
        /// </summary>
        public string GetUnameMachine()
        {
            try
            {
                this.UnameMachine = _processUtil.ExecuteBashShellCommand("uname -m").TrimEnd('\n');
            }
            catch (CommandExecutionFailedException ex)
            {
                SimpleLogger.Error($"Couldn't resolve OS architecture, error=[{ex.Error}");
                throw;
            }

            return this.UnameMachine;
        }

        /// <summary>
        /// The actual implementation of GetEvents for the particular generator, converts audit events to security events
        /// </summary>
        /// <param name="auditEvents">List of audit events according to which security events will be generated</param>
        /// <returns>List of security events</returns>
        protected abstract IEnumerable<IEvent> GetEventsImpl(IEnumerable<AuditEvent> auditEvents);

        private string ExecuteAusearchWithFallback()
        {
            try
            {
                return _processUtil.ExecuteBashShellCommand(GetAusearchCommand(isFallBack: false), AusearchErrorHandler);
            }
            catch (CommandExecutionFailedException ex)
            {
                if (ex.ExitCode == AusearchInvalidCheckpointData ||
                    ex.ExitCode == AusearchCheckpointProcessingError ||
                    ex.ExitCode == AusearchCheckpointEventNotFoundInLog)
                {
                    return _processUtil.ExecuteBashShellCommand(GetAusearchCommand(isFallBack: true), AusearchErrorHandler);
                }

                throw;
            }
        }

        private ErrorHandlerResult AusearchErrorHandler(int errorCode, string errorMessage, string command)
        {
            //If error message indicates no matches than consider error as handled
            return errorMessage == AusearchNoMatchesText
                ? ErrorHandlerResult.ErrorHandled
                : ErrorHandlerResult.ErrorNotHandled;
        }

        private string GetAusearchCommand(bool isFallBack)
        {
            string timeStartArgument = isFallBack ? "-ts checkpoint" : "";

            return $"sudo ausearch {RecordFilteringArguments} --input-logs {timeStartArgument} --checkpoint /var/tmp/{GetType().Name}Checkpoint";
        }
    }
}