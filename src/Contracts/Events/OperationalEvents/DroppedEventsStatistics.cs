// <copyright file="DroppedEventsStatistics.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents
{
    /// <summary>
    /// Agent telemetry dropped events statistics
    /// </summary>
    public class DroppedEventsStatistics : AgentTelemetry<DroppedEventsStatisticsPayload>
    {
        /// <inheritdoc />
        public DroppedEventsStatistics(DroppedEventsStatisticsPayload[] payload) : base(payload)
        {
        }
    }
}
