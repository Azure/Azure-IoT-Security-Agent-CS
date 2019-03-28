// <copyright file="FakeOperationalEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <inheritdoc />
    public class FakeOperationalEvent : EventBase<FakeEventPayload>
    {
        /// <inheritdoc />
        public override EventType EventType { get; } = EventType.Operational;

        /// <inheritdoc />
        public override EventCategory Category { get; } = EventCategory.Periodic;

        /// <inheritdoc />
        // Note: The event priority of this event can be achieved by calling: AgentConfiguration.GetEventPriority<Heartbeat>
        public FakeOperationalEvent(FakeEventPayload payload) : base(EventPriority.Operational, payload)
        {
        }
    }
}
