// <copyright file="Event.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <summary>
    /// Event for the test project, the event is defined according to the IEvent contract
    /// </summary>
    public class Event : IEvent
    {
        /// <inheritdoc cref="Name"/>
        public string Name { get; set; }

        /// <inheritdoc cref="EventType"/>
        public EventType EventType { get; set; }

        /// <inheritdoc cref="IsEmpty"/>
        public bool IsEmpty { get; set; }

        /// <inheritdoc cref="PayloadSchemaVersion"/>
        public string PayloadSchemaVersion { get; set; }

        /// <inheritdoc cref="Id"/>
        public Guid Id { get; set; }

        /// <inheritdoc cref="Priority"/>
        public EventPriority Priority { get; set; }

        /// <inheritdoc cref="Category"/>
        public EventCategory Category { get; set; }

        /// <inheritdoc cref="TimestampLocal"/>
        public DateTime TimestampLocal { get; set; }

        /// <inheritdoc cref="TimestampUTC"/>
        public DateTime TimestampUTC { get; set; }

        /// <inheritdoc cref="EstimatedSize"/>
        public uint EstimatedSize { get; set; }

        /// <inheritdoc cref="Payload"/>
        public IEnumerable<FakeEventPayload> Payload { get; set; }
    }
}