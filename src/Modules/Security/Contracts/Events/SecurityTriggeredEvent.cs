// <copyright file="SecurityTriggeredEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events
{
    /// <summary>
    /// A base class for all security triggered events snapshot events
    /// </summary>
    /// <typeparam name="T">event payloadd</typeparam>
    public abstract class SecurityTriggeredEvent<T> : TriggeredEvent<T>
        where T : Payload, new()
    {
        /// <inheritdoc />
        public override EventType EventType { get; } = EventType.Security;

        /// <inheritdoc />
        protected SecurityTriggeredEvent(EventPriority priority, T payload) : base(priority, payload)
        {
        }
    }
}