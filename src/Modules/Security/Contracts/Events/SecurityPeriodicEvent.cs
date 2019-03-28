// <copyright file="SecurityPeriodicEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events
{
    /// <summary>
    /// A base class for all security snapshot events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SecurityPeriodicEvent<T> : PeriodicEvent<T>
        where T : Payload, new()
    {
        /// <inheritdoc />
        public override EventType EventType { get; } = EventType.Security;

        /// <inheritdoc />
        protected SecurityPeriodicEvent(EventPriority priority, params T[] payloads)
            : base(priority, payloads)
        {
        }
    }
}
