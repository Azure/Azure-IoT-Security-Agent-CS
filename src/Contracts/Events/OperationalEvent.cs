// <copyright file="OperationalEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// A base class for all operational events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class OperationalEvent<T> : EventBase<T>
        where T : Payload, new()
    {
        /// <inheritdoc />
        public override EventType EventType => EventType.Operational;

        /// <summary>
        /// Ctor - creates a new operational event object
        /// </summary>
        /// <param name="payload">The payload of the event</param>
        protected OperationalEvent(T[] payload)
            : base(EventPriority.Operational, payload)
        {
        }
    }
}
