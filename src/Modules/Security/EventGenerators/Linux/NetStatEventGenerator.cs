// <copyright file="NetstatEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.Common.Utils;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Run the netstat command and create events according to its output
    /// Currently only event of OpenPorts is created from all the sockets in LISTEN state
    /// </summary>
    public class NetstatEventGenerator : SnapshotEventGenerator
    {
        private const int LocalAddressColumnNumber = 3;
        private const int RemoteAddressColumnNumber = 4;
        private const int PidColumnNumber = 6;

        private readonly IProcessUtil _processUtil;

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ListeningPorts>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public NetstatEventGenerator() : this(ProcessUtil.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public NetstatEventGenerator(IProcessUtil processUtil)
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
            const string netstatCommand = "sudo netstat -tuwpna"; // -tcp -udp -raw -pid -numeric -all
            string content = _processUtil.ExecuteBashShellCommand(netstatCommand);
            List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(content, LocalAddressColumnNumber, RemoteAddressColumnNumber, PidColumnNumber);

            SimpleLogger.Debug($"NetstatEventGenerator returns {payloads.Count} payloads");

            var openPorts = new ListeningPorts(Priority, payloads.ToArray());
            return new List<IEvent>() { openPorts };
        }
    }
}