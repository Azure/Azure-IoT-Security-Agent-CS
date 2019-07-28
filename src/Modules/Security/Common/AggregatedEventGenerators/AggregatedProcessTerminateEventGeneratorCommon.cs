// <copyright file="AggregatedProcessTerminateEventGeneratorCommon.cs" company="Microsoft">
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
    public class AggregatedProcessTerminateEventGeneratorCommon : AggregatedEventsGenerator<ProcessTerminationPayload>
    {
        /// <inheritdoc />
        protected override TimeSpan AggregationInterval => ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration).ProcessTerminateAggregationInterval;

        /// <inheritdoc />
        protected override bool AggregationEnabled => ((RemoteSecurityModuleConfiguration)AgentConfiguration.RemoteConfiguration).ProcessTerminateAggregationEnabled;

        /// <inheritdoc />
        protected override IEqualityComparer<ProcessTerminationPayload> PayloadComparer { get; } = new ProcessTerminateComparer();

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ProcessTerminate>();

        /// <inheritdoc />
        protected override AggregatedEvent<ProcessTerminationPayload> CreateAggregatedEvent(
            ProcessTerminationPayload payload, int hitCount, DateTime startTime, DateTime endTime)
        {
            payload.ProcessId = 0;

            return new AggregatedEvent<ProcessTerminationPayload>(EventType.Security, Priority, 
                nameof(ProcessTerminate), payload, hitCount, startTime, endTime);
        }
        
        /// <inheritdoc />
        public AggregatedProcessTerminateEventGeneratorCommon(EventGenerator generator)
            : base(generator) { }

        internal sealed class ProcessTerminateComparer : IEqualityComparer<ProcessTerminationPayload>
        {
            /// <inheritdoc />
            public bool Equals(ProcessTerminationPayload p1, ProcessTerminationPayload p2)
            {
                return p1.Executable == p2.Executable
                       && p1.ExitStatus == p2.ExitStatus;
            }

            /// <inheritdoc />
            public int GetHashCode(ProcessTerminationPayload p1)
            {
                return p1.Executable.GetHashCode() ^ p1.ExitStatus.GetHashCode();
            }
        }
    }
}
