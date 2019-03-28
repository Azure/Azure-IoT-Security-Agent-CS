// <copyright file="ConnectionCreate.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class ConnectionCreate : SecurityTriggeredEvent<ConnectionsPayload>
    {
        /// <inheritdoc />
        public override DateTime EventGenerationTime { get; }

        /// <summary>
        /// Get an instance of a connection event
        /// </summary>
        /// <param name="priority">The priority of the event</param>
        /// <param name="payload">The payload of the event</param>
        /// <param name="eventTime">The time the original connection attempt was made</param>
        public ConnectionCreate(EventPriority priority, ConnectionsPayload payload, DateTime eventTime)
            : base(priority, payload)
        {
            EventGenerationTime = eventTime;
        }
    }
}