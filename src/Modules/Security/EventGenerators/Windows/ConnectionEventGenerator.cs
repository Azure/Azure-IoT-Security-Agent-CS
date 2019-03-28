// <copyright file="ConnectionEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Management;
using static Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils.SystemEventUtils;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.ConnectionsPayload;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Event generator for windows connection events
    /// </summary>
    public class ConnectionEventGenerator : ETWEventGeneratorBase
    {
        /// <inheritdoc />
        protected override IEnumerable<string> PrerequisiteCommands => new[] { "auditpol /set /subcategory:\"Filtering Platform Connection\" /success:enable /failure:enable" };

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ConnectionCreate>();

        /// <inheritdoc />
        protected override IEnumerable<ETWEventType> ETWEvents => new[] { ETWEventType.ConnectionAllowed };

        /// <inheritdoc />
        protected override Dictionary<ETWEventType, EtwToIEventConverter> EtwToIotEventConverters => new Dictionary<ETWEventType, EtwToIEventConverter>()
        {
            { ETWEventType.ConnectionAllowed, GetConnectionCreationEvent },
        };

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default WmiUtils and ProcessUtil
        /// </summary>
        public ConnectionEventGenerator() : this(ProcessUtil.Instance, WmiUtils.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public ConnectionEventGenerator(IProcessUtil processUtil, IWmiUtils wmiUtils) : base(processUtil, wmiUtils)
        {
        }

        private IEvent GetConnectionCreationEvent(Dictionary<string, string> ev)
        {
            DateTime eventTime = DateTime.Parse(ev[TimeGeneratedFieldName]);
            ConnectionDirection direction = GetEventPropertyFromMessage(ev[MessageFieldName], DirectionFieldName) == "Outbound" ? ConnectionDirection.Out : ConnectionDirection.In;
            int protocolNumber = Int32.Parse(GetEventPropertyFromMessage(ev[MessageFieldName], ProtocolFieldName));
            FirewallRuleProtocol protocol = (FirewallRuleProtocol)protocolNumber;

            var payload = new ConnectionsPayload
            {
                Executable = GetEventPropertyFromMessage(ev[MessageFieldName], ApplicationNameFieldName),
                ProcessId = UInt32.Parse(GetEventPropertyFromMessage(ev[MessageFieldName], ProcessIdFieldName)),
                CommandLine = null,
                UserId = null,
                RemoteAddress = direction == ConnectionDirection.In ? GetEventPropertyFromMessage(ev[MessageFieldName], SourceAddressFieldName) : GetEventPropertyFromMessage(ev[MessageFieldName], DestinationAddressFieldName),
                RemotePort = direction == ConnectionDirection.In ? GetEventPropertyFromMessage(ev[MessageFieldName], SourcePortFieldName) : GetEventPropertyFromMessage(ev[MessageFieldName], DestinationPortFieldName),
                LocalAddress = direction == ConnectionDirection.Out ? GetEventPropertyFromMessage(ev[MessageFieldName], SourceAddressFieldName) : GetEventPropertyFromMessage(ev[MessageFieldName], DestinationAddressFieldName),
                LocalPort = direction == ConnectionDirection.Out ? GetEventPropertyFromMessage(ev[MessageFieldName], SourcePortFieldName) : GetEventPropertyFromMessage(ev[MessageFieldName], DestinationPortFieldName),
                Protocol = protocol.ToString(),
                Direction = direction
            };

            return new ConnectionCreate(Priority, payload, eventTime);
        }
    }
}
