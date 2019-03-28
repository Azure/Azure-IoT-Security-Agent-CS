// <copyright file="IEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Event interface
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The name of the event
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of the event
        /// </summary>
        EventType EventType { get; }

        /// <summary>
        /// Whether this event is empty
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// The schema version that this event implements
        /// </summary>
        string PayloadSchemaVersion { get; }

        /// <summary>
        /// The ID of the event
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the event priority
        /// </summary>
        [JsonIgnore]
        EventPriority Priority { get; }

        /// <summary>
        /// Gets the event category (real-time or snapshot)
        /// </summary>
        EventCategory Category { get; }

        /// <summary>
        /// Gets the local event generation time
        /// </summary>
        DateTime TimestampLocal { get; }

        /// <summary>
        /// Gets the UTC event generation time
        /// </summary>
        DateTime TimestampUTC { get; }

        /// <summary>
        /// The estimated size of the object (the size of its json serialization)
        /// </summary>
        [JsonIgnore]
        uint EstimatedSize { get; }
    }
}