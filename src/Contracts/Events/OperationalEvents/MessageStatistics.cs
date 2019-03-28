// <copyright file="MessageStatistics.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents
{
    /// <summary>
    /// Message statistics agent telemetry event
    /// </summary>
    public class MessageStatistics : AgentTelemetry<MessageStatisticsPayload>
    {
        /// <inheritdoc />
        public MessageStatistics(MessageStatisticsPayload[] payload) : base(payload)
        {
        }
    }
}
