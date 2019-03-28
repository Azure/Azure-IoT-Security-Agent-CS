// <copyright file="FakePeriodicEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <inheritdoc />
    public class FakePeriodicEvent : PeriodicEvent<FakeEventPayload>
    {
        /// <inheritdoc />
       // Note: The event priority of this event can be achieved by calling: AgentConfiguration.GetEventPriority<ConnectionCreate>
        public FakePeriodicEvent(EventPriority priority, params FakeEventPayload[] payloads) : base(priority, payloads)
        {
        }

        /// <inheritdoc />
        public override EventType EventType =>  EventType.Security;
    }
}

