// <copyright file="SystemInfoEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// System Information event generator class
    /// </summary>
    public class SystemInfoEventGenerator : SnapshotEventGenerator
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<SystemInformation>();

        /// <inheritdoc />
        protected override List<IEvent> GetEventsImpl()
        {
            SystemInformationPayload payload = new SystemInformationPayload
            {
                OSName = "Linux",
                OSVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                OsArchitecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString(),
                HostName = Environment.MachineName
            };

            SimpleLogger.Debug($"SystemInfoEventGenerator returns 1 payload");

            return new List<IEvent> { new SystemInformation(Priority, payload) };
        }
    }
}