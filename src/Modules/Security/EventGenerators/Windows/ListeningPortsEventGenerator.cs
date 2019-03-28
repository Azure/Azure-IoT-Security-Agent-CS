// <copyright file="ListeningPortsEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>


using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.Common.Utils;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Run the netstat command and create events according to its output
    /// </summary>
    public class ListeningPortsEventGenerator : SnapshotEventGenerator
    {
        private readonly IProcessUtil _processUtil;
        private const int LocalAddressColumnNumber = 1;
        private const int RemoteAddressColumnNumber = 2;

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ListeningPorts>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public ListeningPortsEventGenerator() : this (ProcessUtil.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public ListeningPortsEventGenerator(IProcessUtil processUtil)
        {
            _processUtil = processUtil;
        }

        /// <summary>
        /// Read the netsat output
        /// Create an event that conatins all the open ports in state LISTEN (UDP and TCP)
        /// </summary>
        /// <returns>List of open ports event</returns>
        protected override List<IEvent> GetEventsImpl()
        {
            //Run netstat and parse the output
            //We redirect stderr to /dev/null to avoid root requirements (sudo) 
            const string netstatCommand = "netstat -an";
            string content = _processUtil.ExecuteWindowsCommand(netstatCommand);
            List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(content, LocalAddressColumnNumber, RemoteAddressColumnNumber);

            SimpleLogger.Debug($"NetstatEventGenerator returns {payloads.Count} payloads");

            var openPorts = new ListeningPorts(Priority, payloads.ToArray());
            return new List<IEvent>() { openPorts };
        }
    }
}
