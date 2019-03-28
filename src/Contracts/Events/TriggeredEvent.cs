// <copyright file="SecurityEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// A base class for all security events
    /// </summary>
    public abstract class TriggeredEvent<T> : EventBase<T>
        where T : Payload, new()
    {
        /// <inheritdoc/>
        public override EventCategory Category => EventCategory.Triggered;
        
        /// <summary>
        /// Ctor - creates a new triggered event object
        /// </summary>
        /// <param name="priority">The event priority</param>
        /// <param name="payload">The payload of the event</param>
        protected TriggeredEvent(EventPriority priority, T payload)
            : base(priority, payload)
        {
        }
    }
}