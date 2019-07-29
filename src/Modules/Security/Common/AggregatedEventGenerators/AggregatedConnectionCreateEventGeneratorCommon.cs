// <copyright file="AggregatedConnectionCreateEventGeneratorCommon.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Agent.Common.AggregatedEventGenerators
{
    /// <inheritdoc />
    public class AggregatedConnectionCreateEventGeneratorCommon : AggregatedEventsGenerator<ConnectionsPayload>
    {
        /// <inheritdoc />
        protected override TimeSpan AggregationInterval => ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration).ConnectionCreateAgrregationInterval;

        /// <inheritdoc />
        protected override bool AggregationEnabled => ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration).ConnectionCreateAgrregationEnabled;
     
        /// <inheritdoc />
        protected override IEqualityComparer<ConnectionsPayload> PayloadComparer { get; } = new ConnectionCreateComparer();

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ConnectionCreate>();

        /// <inheritdoc />
        protected override AggregatedEvent<ConnectionsPayload> CreateAggregatedEvent(
            ConnectionsPayload payload, int hitCount, DateTime startTime, DateTime endTime)
        {
            if (payload.Direction == ConnectionsPayload.ConnectionDirection.In)
                payload.RemotePort= string.Empty;
            else
                payload.LocalPort= string.Empty;

            payload.ProcessId = 0;

            return new AggregatedEvent<ConnectionsPayload>(EventType.Security, Priority, 
                nameof(ConnectionCreate), payload, hitCount, startTime, endTime);
        }

        /// <inheritdoc />
        public AggregatedConnectionCreateEventGeneratorCommon(params EventGenerator[] generator)
            : base(generator) { }

        internal sealed class ConnectionCreateComparer : IEqualityComparer<ConnectionsPayload>
        {
            /// <inheritdoc />
            public bool Equals(ConnectionsPayload p1, ConnectionsPayload p2)
            {
                return p1.CommandLine == p2.CommandLine
                       && p1.Executable == p2.Executable
                       && p1.UserId == p2.UserId
                       && p1.Direction == p2.Direction
                       && p1.LocalAddress == p2.LocalAddress
                       && p1.RemoteAddress == p2.RemoteAddress
                       && p1.Protocol == p2.Protocol
                       && (p1.Direction == ConnectionsPayload.ConnectionDirection.In && p1.LocalPort == p2.LocalPort
                           || p1.Direction == ConnectionsPayload.ConnectionDirection.Out && p1.RemotePort == p2.RemotePort);
            }

            /// <inheritdoc />
            public int GetHashCode(ConnectionsPayload p1)
            {
                int hashCode = 0;
                hashCode ^= p1.CommandLine?.GetHashCode() ?? 0;
                hashCode ^= p1.Executable?.GetHashCode() ?? 0;
                hashCode ^= p1.UserId?.GetHashCode() ?? 0;
                hashCode ^= p1.Direction.GetHashCode();
                hashCode ^= p1.LocalAddress?.GetHashCode() ?? 0;
                hashCode ^= p1.RemoteAddress?.GetHashCode() ?? 0;
                hashCode ^= p1.Protocol?.GetHashCode() ?? 0;

                return hashCode;
            }
        }
    }
}
