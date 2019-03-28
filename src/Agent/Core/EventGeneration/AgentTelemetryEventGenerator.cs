// <copyright file="AgentTelemetryEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.AgentTelemetry;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// Event generator for agent telemetry
    /// </summary>
    public class AgentTelemetryEventGenerator : SnapshotEventGenerator
    {
        /// <inheritdoc />
        // priority does not matter for operational events
        public override EventPriority Priority { get; } = EventPriority.Operational;

        /// <inheritdoc />
        protected override List<IEvent> GetEventsImpl()
        {
            var data = TelemetryCollector.GetDataAndReset();
            return GenerateTelemetryEventsFromCounterData(data);
        }

        /// <summary>
        /// Generates telemetry events from the given data
        /// </summary>
        /// <param name="counterData"></param>
        /// <returns></returns>
        private List<IEvent> GenerateTelemetryEventsFromCounterData(IDictionary<CounterType, int> counterData)
        {

            var messageStatsEvent = new MessageStatistics(new[]
            {
                new MessageStatisticsPayload
                {
                    MessagesUnder4KB = counterData[CounterType.MessagesUnder4KB],
                    TotalFailed = counterData[CounterType.SendFailed],
                    MessagesSent = counterData[CounterType.SendSuccesfully] + counterData[CounterType.SendFailed]
                }
            });

            var droppedEventsEvent = new DroppedEventsStatistics(new[]
            {
                new DroppedEventsStatisticsPayload
                {
                    Queue = EventPriority.High,
                    CollectedEvents = counterData[CounterType.EnqueuedHighPriorityEvent],
                    DroppedEvents = counterData[CounterType.DroppedHighPriorityEvent]
                },
                new DroppedEventsStatisticsPayload
                {
                    Queue = EventPriority.Low,
                    CollectedEvents = counterData[CounterType.EnqueuedLowPriorityEvent],
                    DroppedEvents = counterData[CounterType.DroppedLowPriorityEvent]
                },
                new DroppedEventsStatisticsPayload
                {
                    Queue = EventPriority.Operational,
                    CollectedEvents = counterData[CounterType.EnqueuedOperationalEvent],
                    DroppedEvents = counterData[CounterType.DroppedOperationalEvent]
                }
            });

            return new List<IEvent> { messageStatsEvent, droppedEventsEvent };
        }
    }
}
