// <copyright file="EventQueueManager.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.AgentTelemetry;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.MessageWorker
{
    /// <summary>
    /// Event dropped from queue system notification
    /// </summary>
    /// <param name="priority">event priority</param>
    /// <param name="amount">amount of events</param>
    public delegate void EventDropped(EventPriority priority, int amount = 1);

    /// <summary>
    /// Event enqueued to queue system notification
    /// </summary>
    /// <param name="priority">event priority</param>
    /// <param name="amount">amount of events</param>
    public delegate void EventEnqueued(EventPriority priority, int amount = 1);

    /// <summary>
    /// Manages multiple event queues with different priorities
    /// </summary>
    public class EventQueueManager
    {
        /// <summary>
        /// The event queues by event priority
        /// </summary>
        private readonly Dictionary<EventPriority, EventQueue> _eventQueues;

        private static Lazy<EventQueueManager> _instance = new Lazy<EventQueueManager>(() => new EventQueueManager());

        /// <summary>
        /// The instance of the EventQueueManager singleton
        /// </summary>
        public static EventQueueManager Instance => _instance.Value;

        /// <summary>
        ///  The fraction of size from the total amount of space allocated for the agent queues
        /// </summary>
        private const double OperationalQueueSizePercentage = 0.1;

        /// <summary>
        /// Event dropped from queue notification handlers
        /// </summary>
        public static event EventDropped EventDroppedHandlers;

        /// <summary>
        /// Event enqueued notification handlers
        /// </summary>
        public static event EventEnqueued EventEnqueuedHandlers;

        /// <summary>
        /// Resets the instance, used in tests 
        /// </summary>
        public static void ResetInstance()
        {
            if (_instance.IsValueCreated)
                _instance.Value.UnregisterEvents();

            _instance = new Lazy<EventQueueManager>(() => new EventQueueManager());
        }

        private void UnregisterEvents()
        {
            EventDroppedHandlers -= CollectEventDroppedTelemetry;
            EventEnqueuedHandlers -= CollectEventEnqueuedTelemetry;
        }

        /// <summary>
        /// Ctor - initialize a new event queue manager object
        /// </summary>
        private EventQueueManager()
        {
            _eventQueues = new Dictionary<EventPriority, EventQueue>
            {
                { EventPriority.High, new EventQueue(LocalConfiguration.General.HighPriorityQueueSizePercentage) },
                { EventPriority.Low, new EventQueue(LocalConfiguration.General.LowPriorityQueueSizePercentage) },
                { EventPriority.Operational, new EventQueue(OperationalQueueSizePercentage) }
            };

            EventDroppedHandlers += CollectEventDroppedTelemetry;
            EventEnqueuedHandlers += CollectEventEnqueuedTelemetry;
        }

        /// <summary>
        /// Enqueue new events
        /// </summary>
        /// <param name="events">The list of events to add</param>
        public void EnqueueEvents(IEnumerable<IEvent> events)
        {
            foreach (IEvent ev in events)
            {
                _eventQueues[ev.Priority].Enqueue(ev);
            }
        }

        /// <summary>
        /// Returns the amount of available data across all queues
        /// </summary>
        /// <returns>The size of available data</returns>
        public uint AvailableDataSize()
        {
            uint totalSize = 0;
            foreach (EventQueue queue in _eventQueues.Values)
            {
                totalSize += queue.TotalQueueEventSize;
            }

            return totalSize;
        }

        /// <summary>
        /// Dequeue available events from the queue of the given priority up to the max requested size
        /// </summary>
        /// <param name="queuePriority">The priority to start with</param>
        /// <param name="maxSize">The max size to return</param>
        /// <param name="dequeuedEvents">Output of the dequeued events</param>
        /// <returns>The actual size retrieved</returns>
        public uint DequeueFromSingleQueue(EventPriority queuePriority, uint maxSize, out List<IEvent> dequeuedEvents)
        {
            uint currentSize = 0;
            dequeuedEvents = new List<IEvent>();

            if (queuePriority == EventPriority.Off)
            {
                return currentSize;
            }

            while (true)
            {
                var remainedSize = maxSize - currentSize;
                IEvent returnedEvent = _eventQueues[queuePriority].DequeueIf(ev => ev.EstimatedSize <= remainedSize);
                if (returnedEvent == null)
                {
                    break;
                }

                dequeuedEvents.Add(returnedEvent);
                currentSize += returnedEvent.EstimatedSize;
            }

            return currentSize;
        }

        /// <summary>
        /// Dequeue available events, starting with preferred priority and adding from other priority if
        /// there is still space remaining up to the max requested size.
        /// </summary>
        /// <param name="preferredPriority">The priority to start with</param>
        /// <param name="maxSize">The max size to return</param>
        /// <param name="dequeuedEvents">Output of the dequeued events</param>
        /// <returns>The actual size retrieved</returns>
        public uint DequeueEventsFromMultipleQueues(EventPriority preferredPriority, uint maxSize, out List<IEvent> dequeuedEvents)
        {
            dequeuedEvents = new List<IEvent>();
            if (preferredPriority == EventPriority.Off || _eventQueues[preferredPriority].IsEmpty())
            {
                return 0;
            }

            uint currentSize = DequeueFromSingleQueue(EventPriority.Operational, maxSize, out var events);
            dequeuedEvents.AddRange(events);

            currentSize += DequeueFromSingleQueue(preferredPriority, maxSize - currentSize, out events);
            dequeuedEvents.AddRange(events);

            // If we got some events for the message, we try to add more events from the other priority
            if (currentSize > 0)
            {
                var oppositePriority = GetOppositePriority(preferredPriority);
                currentSize += DequeueFromSingleQueue(oppositePriority, maxSize - currentSize, out events);
                dequeuedEvents.AddRange(events);
            }

            return currentSize;
        }

        /// <summary>
        /// Handle event dropped
        /// </summary>
        /// <param name="priority">event priority</param>
        /// <param name="amount">amount of events</param>
        public static void OnEventDropped(EventPriority priority, int amount = 1)
        {
            EventDroppedHandlers?.Invoke(priority, amount);
        }

        /// <summary>
        /// Handle enqueued event notification
        /// </summary>
        /// <param name="priority">event priority</param>
        /// <param name="amount">amount of events</param>
        public static void OnEventEnqueued(EventPriority priority, int amount = 1)
        {
            EventEnqueuedHandlers?.Invoke(priority, amount);
        }

        /// <summary>
        /// Translates the opposite priority (from high to low and from low to high)
        /// </summary>
        /// <param name="priority">The given priority</param>
        /// <returns>The opposite priority</returns>
        private EventPriority GetOppositePriority(EventPriority priority)
        {
            return priority == EventPriority.High ? EventPriority.Low : EventPriority.High;
        }

        private static void CollectEventDroppedTelemetry(EventPriority priority, int amount)
        {
            if (priority == EventPriority.Low)
                CounterType.DroppedLowPriorityEvent.Get().IncrementBy(amount);
            if (priority == EventPriority.High)
                CounterType.DroppedHighPriorityEvent.Get().IncrementBy(amount);
            if (priority == EventPriority.Operational)
                CounterType.DroppedOperationalEvent.Get().IncrementBy(amount);
        }

        private static void CollectEventEnqueuedTelemetry(EventPriority priority, int amount)
        {
            if (priority == EventPriority.Low)
                CounterType.EnqueuedLowPriorityEvent.Get().IncrementBy(amount);
            if (priority == EventPriority.High)
                CounterType.EnqueuedHighPriorityEvent.Get().IncrementBy(amount);
            if (priority == EventPriority.Operational)
                CounterType.EnqueuedOperationalEvent.Get().IncrementBy(amount);
        }
    }
}