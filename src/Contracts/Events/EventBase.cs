// <copyright file="EventBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// A base class for all events: security, snapshot and operational
    /// </summary>
    /// <typeparam name="T">Event payload</typeparam>
    public abstract class EventBase<T> : IEvent
        where T : Payload
    {
        /// <inheritdoc />
        public string Name => GetType().Name;

        /// <inheritdoc />
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract EventType EventType{ get; }

        /// <inheritdoc />
        public virtual bool IsEmpty { get; }

        /// <inheritdoc />
        public virtual string PayloadSchemaVersion { get; } = "1.0";

        /// <inheritdoc />
        public Guid Id { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public EventPriority Priority { get; private set; }

        /// <inheritdoc />
        [JsonConverter(typeof(StringEnumConverter))]
        public abstract EventCategory Category { get; }

        /// <summary>
        /// Gets the event generation time
        /// </summary>
        [JsonIgnore]
        public virtual DateTime EventGenerationTime { get; } = DateTime.Now;

        /// <inheritdoc />
        public DateTime TimestampLocal => EventGenerationTime.ToLocalTime();

        /// <inheritdoc />
        public DateTime TimestampUTC => EventGenerationTime.ToUniversalTime();

        /// <summary>
        /// The estimated size of the object (the size of its json serialization)
        /// </summary>
        public uint EstimatedSize => LazyEstimatedSize.Value;

        /// <summary>
        /// The list of payloads contained in the event
        /// </summary>
        public IEnumerable<T> Payload { get; private set; }

        /// <summary>
        /// Ctor - creates a new event object
        /// </summary>
        /// <param name="priority">The event priority</param>
        /// <param name="payloads">The event payload</param>
        protected EventBase(EventPriority priority, params T[] payloads)
        {
            if (!payloads.Any())
            {
                IsEmpty = true;
            }

            Priority = priority;
            Payload = payloads;
        }

        /// <summary>
        /// Serializes the event into a printable string
        /// </summary>
        /// <returns>The event string representation</returns>
        public override string ToString()
        {
            return $"Event ({TimestampLocal}): P={Priority}, C={Category}, T={Name}, ID={Id}";
        }

        /// <summary>
        /// Using Lazy in order to generate the estimated size only once and only when needed
        /// </summary>
        private Lazy<uint> LazyEstimatedSize => new Lazy<uint>(GetEstimatedSize);

        private uint GetEstimatedSize()
        {
            return (uint) Encoding.Unicode.GetByteCount(JsonConvert.SerializeObject(this,
                JsonFormatting.SerializationSettings));
        }
    }
}