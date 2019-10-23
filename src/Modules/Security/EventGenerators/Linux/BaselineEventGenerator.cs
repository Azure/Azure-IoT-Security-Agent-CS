// <copyright file="BaselineEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.Common;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.OsBaseline;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Baseline event generator for Linux
    /// </summary>
    public class BaselineEventGenerator : SnapshotEventGenerator
    {
        private readonly IProcessUtil _processUtil;

        /// <summary>
        /// The command to execute the baseline rules
        /// </summary>
        private const string BaselineExecCommandTemplate = @"sudo {0}/BaselineExecutables/omsbaseline -d {0}/BaselineExecutables/";

        /// <summary>
        /// The command to execute custom checks baseline
        /// </summary>
        private const string BaselineCustomCheckExecCommandTemplate = @"sudo {0}/BaselineExecutables/omsbaseline -ccfp {1} -ccfh {2}";

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<OSBaseline>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public BaselineEventGenerator() : this (ProcessUtil.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public BaselineEventGenerator(IProcessUtil processUtil)
        {
            _processUtil = processUtil;
        }

        /// <summary>
        /// OMSBaseline custom checks configuration enabled predicate
        /// </summary>
        public static bool IsCustomChecksEnabled()
        {
            RemoteSecurityModuleConfiguration agentConfiguration = ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration);

            return agentConfiguration.BaselineCustomChecksEnabled && 
                !string.IsNullOrWhiteSpace(agentConfiguration.BaselineCustomChecksFilePath) &&
                !string.IsNullOrWhiteSpace(agentConfiguration.BaselineCustomChecksFileHash);
        }

        /// <summary>
        /// OMSBaseline custom checks configuration enabled predicate
        /// </summary>
        protected IEnumerable<BaselinePayload> ExecuteBaseline(string command)
        {
            string output = _processUtil.ExecuteBashShellCommand(command);

            if (string.IsNullOrWhiteSpace(output))
            {
                throw new ApplicationException("attempt to run baseline scan failed");
            }

            BaselineScanOutput deserializedOutput = JsonConvert.DeserializeObject<BaselineScanOutput>(output);

            if (!string.IsNullOrWhiteSpace(deserializedOutput.Error))
            {
                throw new ApplicationException($"baseline scan failed with error: {deserializedOutput.Error}");
            }
            else if (deserializedOutput.Results == null)
            {
                throw new ApplicationException($"baseline results are null");
            }

            deserializedOutput.Results.RemoveAll(result => result.Result == BaselineResult.ResultType.Pass || result.Result == BaselineResult.ResultType.Skip);
            var payloads = deserializedOutput.Results.Select(GetPayloadFromResult);

            SimpleLogger.Debug($"BaselineEventGenerator returns {payloads.Count()} payloads");

            return payloads;
        }

        /// <summary>
        /// Run the baseline scan and get the results as a baseline event
        /// </summary>
        /// <returns>Baseline event</returns>
        protected override List<IEvent> GetEventsImpl()
        {
            // resolve agent directory
            string agentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // default baseline compliance checks payload
            List<BaselinePayload> defaultBaselinePayload = ExecuteBaseline(string.Format(BaselineExecCommandTemplate, agentDirectory)).ToList();

            if (BaselineEventGenerator.IsCustomChecksEnabled())
            {
                try 
                {
                    RemoteSecurityModuleConfiguration agentConfiguration = ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration);

                    IEnumerable<BaselinePayload> customBaselinePayload = ExecuteBaseline(string.Format(
                            BaselineCustomCheckExecCommandTemplate, 
                            agentDirectory,
                            agentConfiguration.BaselineCustomChecksFilePath,
                            agentConfiguration.BaselineCustomChecksFileHash
                        )
                    );

                    defaultBaselinePayload.AddRange(customBaselinePayload);
                }
                catch (CommandExecutionFailedException ex)
                {  
                    SimpleLogger.Error($"BaselineEventGenerator failed to execute custom checks, {ex.Message}");
                }
            }

            OSBaseline osbaselineEvent = new OSBaseline(
                Priority, 
                defaultBaselinePayload.ToArray()
            );

            return new List<IEvent>() { osbaselineEvent };
        }

        /// <summary>
        /// Creates a BaselinePayload from the given BaselineResult 
        /// </summary>
        /// <param name="result">The BaselineResult</param>
        /// <returns>Baseline payload</returns>
        private BaselinePayload GetPayloadFromResult(BaselineResult result)
        {
            return new BaselinePayload
            {
                Result = result.Result.ToString(),
                Error = result.ErrorText,
                CceId = result.CceId,
                Severity = result.Severity.ToString(),
                Description = result.Description
            };
        }
    }
}
