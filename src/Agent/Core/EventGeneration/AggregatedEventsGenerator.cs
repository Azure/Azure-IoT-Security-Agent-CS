// <copyright file="AggregatedEventsGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// Base class for agrregated events events generator
    /// This event generator decorates a "regular" event generator and aggregates the events produced by this event generator
    /// Aggregation type is count agrregation
    /// </summary>
    /// <typeparam name="TPayload">The payload of the aggregated event</typeparam>
    public abstract class AggregatedEventsGenerator<TPayload> : EventGenerator
        where TPayload : Payload
    {
        private readonly EventGenerator[] _eventGenerators;
        private readonly Dictionary<TPayload, int> _aggregatedEvents;
        private DateTime _lastAggregationTime;

        /// <summary>
        /// Is aggregation enabled
        /// </summary>
        protected abstract bool AggregationEnabled { get; }

        /// <summary>
        /// Agrregation interval
        /// </summary>
        protected abstract TimeSpan AggregationInterval { get; }

        /// <summary>
        /// Compares between similar event, agrregation will count equal payloads according to this comparer
        /// </summary>
        protected abstract IEqualityComparer<TPayload> PayloadComparer { get; }

        /// <summary>
        /// Create new aggregated event from the given payload
        /// The method is responsible for cleaning up any non aggregated data
        /// (i.e if aggregation is based on fields x,y remove field z from the event)
        /// </summary>
        /// <param name="payload">the payload to create the event from</param>
        /// <param name="hitCount">payload hit count</param>
        /// <param name="startTime">aggregation start time</param>
        /// <param name="endTime">aggregation end time</param>
        /// <returns>new aggregated event</returns>
        protected abstract AggregatedEvent<TPayload> CreateAggregatedEvent(TPayload payload, int hitCount, DateTime startTime, DateTime endTime);

        /// <summary>
        /// C-tor
        /// </summary>
        /// <param name="eventGenerator">event generator for generating events to be aggregated</param>
        protected AggregatedEventsGenerator(params EventGenerator[] eventGenerator)
        {
            _eventGenerators = eventGenerator;
            _aggregatedEvents = new Dictionary<TPayload, int>(PayloadComparer);
            _lastAggregationTime = DateTime.UtcNow;
        }

        /// <inheritdoc />>
        public override IEnumerable<IEvent> GetEvents()
        {
            IEnumerable<IEvent> collectedEvents = _eventGenerators
                .SelectMany(eg => eg.GetEvents());

            List<IEvent> returnedEvents = new List<IEvent>();
            if (AggregationEnabled)
            {
                Aggregate(collectedEvents.Cast<EventBase<TPayload>>());
            }
            else
            {
                returnedEvents.AddRange(collectedEvents);
            }
            IEnumerable<IEvent> aggregatedEvents = GetAggregatedEvents();
            returnedEvents.AddRange(aggregatedEvents);
            return returnedEvents;
        }

        private IEnumerable<IEvent> GetAggregatedEvents()
        {
            // if aggregation is disabled "flush" events
            if (!AggregationEnabled || DateTime.UtcNow - _lastAggregationTime > AggregationInterval)
            {
                IEnumerable<IEvent> events = CreateEventsFromAggregation();
                _aggregatedEvents.Clear();
                _lastAggregationTime = DateTime.UtcNow;

                return events;
            }

            return new List<IEvent>();
        }

        private IEnumerable<IEvent> CreateEventsFromAggregation()
        {
            var now = DateTime.UtcNow;
            var result = _aggregatedEvents.Select(kvp => CreateAggregatedEvent(kvp.Key, kvp.Value, _lastAggregationTime, now));
            return result.ToList();
        }

        private void Aggregate(IEnumerable<EventBase<TPayload>> events)
        {
            IEnumerable<TPayload> payloads = events.SelectMany(ev => ev.Payload);
            foreach (TPayload payload in payloads)
            {
                _aggregatedEvents.TryGetValue(payload, out int count);
                _aggregatedEvents[payload] = count + 1;
            }
        }
    }
}