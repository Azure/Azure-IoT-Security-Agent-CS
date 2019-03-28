// <copyright file="EventProducer.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.Providers;
using Microsoft.Azure.IoT.Agent.Core.Scheduling;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// Event producer that collects events from all event generators on schedule
    /// and stores them into the shared queue
    /// </summary>
    public class EventProducer : ITask
    {
        /// <summary>
        /// Event generators for be used for event production
        /// </summary>
        private readonly IEventGenerator[] _eventGenerators;

        /// <summary>
        /// Ctor - creates a new event producer object with the given EventGenerator provider
        /// </summary>
        /// <param name="eventGenerators">Event generators to use for event production</param>
        public EventProducer(params IEventGenerator[] eventGenerators)
        {
            _eventGenerators = eventGenerators;
        }

        /// <summary>
        /// The task interface method for executing the event producer work on schedule
        /// </summary>
        public void ExecuteTask()
        {
            foreach (IEventGenerator generator in _eventGenerators)
            {
                if (generator.Priority == EventPriority.Off)
                    continue;

                try
                {
                    // Collect events from current generator
                    IEnumerable<IEvent> events = generator.GetEvents();
                    EventQueueManager.Instance.EnqueueEvents(events);                   
                    SimpleLogger.Debug($"Generator {generator.GetType().Name} returned {events.Count()} events");
                }
                catch (Exception ex)
                {
                    SimpleLogger.Error($"Data collection failed for generator {generator.GetType().Name}", exception: ex);
                }
            }
        }
    }
}