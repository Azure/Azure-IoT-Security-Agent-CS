// <copyright file="AggregatedEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Base type for aggregated events
    /// </summary>
    public class AggregatedEvent<TPayload> : EventBase<TPayload>
        where TPayload : Payload
    {
        /// <inheritdoc/>
        public override string Name { get; }

        /// <inheritdoc/>
        public override EventType EventType { get; }

        /// <inheritdoc/>
        public override EventCategory Category => EventCategory.Aggregated;

        /// <summary>
        /// Creates a new aggregated event based on the given event
        /// </summary>
        /// <param name="payload">event payload</param>
        /// <param name="hitCount">event hit count</param>
        /// <param name="start">aggregation start time</param>
        /// <param name="end">aggregation end time</param>
        /// <param name="type">event type</param>
        /// <param name="priority">event priority</param>
        /// <param name="eventName">event name</param>
        /// <returns>new aggregated event</returns>
        public AggregatedEvent(EventType type, EventPriority priority, string eventName, TPayload payload, int hitCount, DateTime start, DateTime end) 
        : base(priority, payload)
        {
            Name = eventName;
            EventType = type;
            if (payload.ExtraDetails == null)
            {
                payload.ExtraDetails = new Dictionary<string, string>();
            }
            payload.ExtraDetails.Add("StartTimeLocal", start.ToLocalTime().ToString(CultureInfo.InvariantCulture));
            payload.ExtraDetails.Add("StartTimeUtc", start.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
            payload.ExtraDetails.Add("EndTimeLocal", end.ToLocalTime().ToString(CultureInfo.InvariantCulture));
            payload.ExtraDetails.Add("EndTimeUtc", end.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
            payload.ExtraDetails.Add("HitCount", hitCount.ToString());
        }
    }
}
