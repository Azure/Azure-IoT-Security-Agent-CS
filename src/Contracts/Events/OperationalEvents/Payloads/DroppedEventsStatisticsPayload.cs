// <copyright file="DroppedEventsStatisticsPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads
{
    /// <summary>
    /// Dropped events telemetry payload
    /// </summary>
    public class DroppedEventsStatisticsPayload : Payload
    {
        /// <summary>
        /// Queue name
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EventPriority Queue { get; set; }

        /// <summary>
        /// Total events
        /// </summary>
        public int CollectedEvents { get; set; }

        /// <summary>
        /// Total amount of dropped events
        /// </summary>
        public int DroppedEvents { get; set; }
    }
}
