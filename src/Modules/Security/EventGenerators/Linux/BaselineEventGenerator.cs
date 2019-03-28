// <copyright file="BaselineEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
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
        /// The command to execute to run the baseline rules
        /// </summary>
        private const string BaselineExecCommandTemplate = @"sudo {0}/BaselineExecutables/omsbaseline_{1} -d {0}/BaselineExecutables/";

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
        /// Run the baseline scan and get the results as a baseline event
        /// </summary>
        /// <returns>Baseline event</returns>
        protected override List<IEvent> GetEventsImpl()
        {
            string agentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string bitnessSuffix = RuntimeInformation.OSArchitecture == Architecture.Arm64 || RuntimeInformation.OSArchitecture == Architecture.X64 ? "x64" : "x86";
            string output = _processUtil.ExecuteBashShellCommand(string.Format(BaselineExecCommandTemplate, agentDirectory, bitnessSuffix));

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

            var ev = new OSBaseline(Priority, payloads.ToArray());
            return new List<IEvent> { ev };
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
