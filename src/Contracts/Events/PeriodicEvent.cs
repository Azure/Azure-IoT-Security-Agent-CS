// <copyright file="PeriodicEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// A base class for all snapshot events
    /// </summary>
    public abstract class PeriodicEvent<T> : EventBase<T>
        where T : Payload, new()
    {
        /// <inheritdoc/>
        public override EventCategory Category => EventCategory.Periodic;

        /// <summary>
        /// Ctor - creates a new periodic event object
        /// </summary>
        /// <param name="priority">The event priority</param>
        /// <param name="payloads">The payloads of the event</param>
        protected PeriodicEvent(EventPriority priority, params T[] payloads)
            : base( priority, payloads)
        {
        }
    }
}