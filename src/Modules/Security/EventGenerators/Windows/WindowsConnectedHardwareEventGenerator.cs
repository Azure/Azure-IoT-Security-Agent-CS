// <copyright file="WindowsConnectedHardwareEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Generates  the connected hardware snapshot event on Windows
    /// </summary>
    public class WindowsConnectedHardwareEventGenerator : SnapshotEventGenerator
    {
        private readonly IWmiUtils _wmiUtils;

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ConnectedHardware>();

        /// <summary>
        /// C-tor creates a new instance.
        /// use default WmiUtils
        /// </summary>
        public WindowsConnectedHardwareEventGenerator() : this(WmiUtils.Instance) { }

        /// <summary>
        /// C-tor creates a new instance.
        /// </summary>
        public WindowsConnectedHardwareEventGenerator(IWmiUtils wmiUtils)
        {
            _wmiUtils = wmiUtils;
        }

        /// <inheritdoc />
        protected override List<IEvent> GetEventsImpl()
        {
            var connectedHardware = _wmiUtils.GetEnumerableValue(WmiDataType.ConnectedDevices);
            ConnectedHardwarePayload[] payloads = connectedHardware
                .Select(item => new ConnectedHardwarePayload { ConnectedHardware = item }).ToArray();

            var ev = new ConnectedHardware(Priority, payloads);
            return new List<IEvent> { ev };
        }
    }
}