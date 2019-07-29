// <copyright file="AggregatedProcessCreateEventGeneratorCommon.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Agent.Common.AggregatedEventGenerators
{
    /// <inheritdoc />
    public class AggregatedProcessCreateEventGeneratorCommon : AggregatedEventsGenerator<ProcessCreationPayload>
    {
        /// <inheritdoc />
        protected override TimeSpan AggregationInterval => ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration).ProcessCreateAgrregationInterval;

        /// <inheritdoc />
        protected override bool AggregationEnabled => ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration).ProcessCreateAgrregationEnabled;

        /// <inheritdoc />
        protected override IEqualityComparer<ProcessCreationPayload> PayloadComparer { get; } = new ProcessCreateComparer();

        /// <inheritdoc />s
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ProcessCreate>();

        /// <inheritdoc />
        protected override AggregatedEvent<ProcessCreationPayload> CreateAggregatedEvent(
            ProcessCreationPayload payload, int hitCount, DateTime startTime, DateTime endTime)
        {
            payload.ParentProcessId = 0;
            payload.ProcessId = 0;

            return new AggregatedEvent<ProcessCreationPayload>(EventType.Security, Priority, 
                nameof(ProcessCreate), payload, hitCount, startTime, endTime);
        }

        /// <inheritdoc />
        public AggregatedProcessCreateEventGeneratorCommon(EventGenerator generator)
            : base(generator) { }

        internal sealed class ProcessCreateComparer : IEqualityComparer<ProcessCreationPayload>
        {
            /// <inheritdoc />
            public bool Equals(ProcessCreationPayload p1, ProcessCreationPayload p2)
            {
                return p1.CommandLine == p2.CommandLine
                       && p1.Executable == p2.Executable
                       && p1.UserId == p2.UserId
                       && p1.UserName == p2.UserName;
            }

            /// <inheritdoc />
            public int GetHashCode(ProcessCreationPayload p1)
            {
                int hashCode = 0;
                hashCode ^= p1.CommandLine?.GetHashCode() ?? 0;
                hashCode ^= p1.Executable?.GetHashCode() ?? 0;
                hashCode ^= p1.UserId?.GetHashCode() ?? 0;
                hashCode ^= p1.UserName?.GetHashCode() ?? 0;
                return hashCode;
            }
        }
    }
}
