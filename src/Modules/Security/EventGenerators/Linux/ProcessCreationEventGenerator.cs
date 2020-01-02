// <copyright file="ProcessCreationEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Process creation event generator for Linux
    /// </summary>
    public class ProcessCreationEventGenerator : AuditEventGeneratorBase
    {
        private Dictionary<string,string> executableHash;
        private string searchIntegrityRuleCommand = "sudo ausearch -m INTEGRITY_RULE --input-logs";
        /// <inheritdoc />
        protected override IEnumerable<string> AuditRulesPrerequisites { get; } = new List<string>()
        {
            "-A always,exit -F arch={0} -S execve,execveat"
        };

        /// <inheritdoc />
        protected override IEnumerable<AuditEventType> AuditEventTypes => new[] { AuditEventType.ProcessExecution, AuditEventType.Integrity };

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ProcessCreate>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public ProcessCreationEventGenerator() : this(ProcessUtil.Instance) { }

        /// <inheritdoc />
        public ProcessCreationEventGenerator(IProcessUtil processUtil) : base(processUtil)
        {
            executableHash = new Dictionary<string,string>();
            string searchResultsString = processUtil.ExecuteBashShellCommand(searchIntegrityRuleCommand);
            IEnumerable<string> searchResults = searchResultsString.Split("----", StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<AuditEvent> auditEvents = searchResults.Select(AuditEvent.ParseFromAusearchLine);

            foreach(AuditEvent auditEvent in auditEvents){
                updateExecutablesDictonary(auditEvent);
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<IEvent> GetEventsImpl(IEnumerable<AuditEvent> auditEvents)
        {
            foreach(AuditEvent auditEvent in auditEvents){
                updateExecutablesDictonary(auditEvent);
            }
            return auditEvents.Select(AuditEventToDeviceEvent);
        }

        private void updateExecutablesDictonary(AuditEvent auditEvent){
            var hash = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ExecutableHash, false);
            if(hash != null){
                hash = hash.Substring(7);
                var exec = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.FilePath);
                executableHash[exec] = hash;
            }
        }
        /// <summary>
        /// Converts an audit event to a device event
        /// </summary>
        /// <param name="auditEvent">Audit event to convert</param>
        /// <returns>Device event based on the input</returns>
        private IEvent AuditEventToDeviceEvent(AuditEvent auditEvent)
        {
            var executable = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Executable);
            bool isExecutableExist = executableHash.TryGetValue(executable, out string hash);
            hash =  isExecutableExist ? hash : "";

            ProcessCreationPayload payload = new ProcessCreationPayload
            {
                CommandLine = EncodedAuditFieldsUtils.DecodeHexStringIfNeeded(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.CommandLine), Encoding.UTF8),
                Executable = executable,
                ProcessId = Convert.ToUInt32(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ProcessId)),
                ParentProcessId = Convert.ToUInt32(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ParentProcessId)),
                Time = auditEvent.TimeUTC,
                UserId = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.UserId),
                ExtraDetails = new Dictionary<string,string>(){ {"Hash", hash} }
            };
            return new ProcessCreate(Priority, payload);
        }
    }
}