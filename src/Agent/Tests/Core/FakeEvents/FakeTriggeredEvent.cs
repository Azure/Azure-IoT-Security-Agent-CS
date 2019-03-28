// <copyright file="FakeTriggeredEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <inheritdoc />
    public class FakeTriggeredEvent : TriggeredEvent<FakeEventPayload>
    {
        /// <inheritdoc />
        // Note: The event priority of this event can be achieved by calling: AgentConfiguration.GetEventPriority<ProcessCreate>
        public FakeTriggeredEvent(EventPriority priority, FakeEventPayload payload) : base(priority, payload)
        {
        }

        /// <inheritdoc />
        public override EventType EventType => EventType.Security;
    }
}