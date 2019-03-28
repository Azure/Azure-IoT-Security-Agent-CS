// <copyright file="EventQueue.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.MessageWorker
{
    /// <summary>
    /// Stores a queue of events with a maximal size
    /// </summary>
    public class EventQueue
    {
        /// <summary>
        /// Gets the accumulated size of all the events stored in the queue
        /// </summary>
        public uint TotalQueueEventSize { get; private set; }

        /// <summary>
        /// The maximal allowed accumulated size of all the events in the queue
        /// </summary>
        private uint MaxQueueEventSize => (uint)(AgentConfiguration.RemoteConfiguration.MaxLocalCacheSize * _precentageOfTotalQueueSize);

        /// <summary>
        /// The internal queue that holds the events
        /// </summary>
        private readonly Queue<IEvent> _internalQueue = new Queue<IEvent>();

        /// <summary>
        /// The fractional amount of memory out of the total memory allocated for the queues, that this queue is allowed to consume
        /// </summary>
        private readonly double _precentageOfTotalQueueSize;

        /// <summary>
        /// Lock object for access to the queue
        /// </summary>
        private readonly object _queueLock = new object();

        /// <summary>
        /// Ctor - creates a new event queue object
        /// </summary>
        /// <param name="precentageOfTotalQueueSize">
        /// The fraction of size from the total amount of space allocated for the queues that
        /// this queue should occupy
        /// </param>
        public EventQueue(double precentageOfTotalQueueSize)
        {
            TotalQueueEventSize = 0;
            _precentageOfTotalQueueSize = precentageOfTotalQueueSize;
        }

        /// <summary>
        /// Add new event to the queue
        /// </summary>
        /// <param name="ev">The event to add</param>
        public void Enqueue(IEvent ev)
        {
            lock (_queueLock)
            {
                if (ev.EstimatedSize > MaxQueueEventSize || ev.EstimatedSize > AgentConfiguration.RemoteConfiguration.MaxMessageSize)
                {
                    SimpleLogger.Warning($"Not adding event {ev.Name} because maxLocalCacheSize is too small. MaxQueueEventSize: {MaxQueueEventSize} EventPriority {ev.EstimatedSize} Max message size: {AgentConfiguration.RemoteConfiguration.MaxMessageSize}");
                    EventQueueManager.OnEventEnqueued(ev.Priority);
                    EventQueueManager.OnEventDropped(ev.Priority);
                       return;
                }

                _internalQueue.Enqueue(ev);
                TotalQueueEventSize += ev.EstimatedSize;
                EventQueueManager.OnEventEnqueued(ev.Priority);

                // Remove surplus events
                int removedEvents = 0;
                while (TotalQueueEventSize > MaxQueueEventSize)
                {
                    IEvent removedEvent = UnsafeDequeue();
                    if (removedEvent == null)
                    {
                        break;
                    }

                    SimpleLogger.Warning("Removing event: " + removedEvent);
                    removedEvents++;
                }
                EventQueueManager.OnEventDropped(ev.Priority, removedEvents);
            }
        }

        /// <summary>
        /// Dequeue a single event
        /// </summary>
        /// <returns>The removed event or null if no event is available</returns>
        public IEvent Dequeue()
        {
            lock (_queueLock)
            {
                return UnsafeDequeue();
            }
        }

        /// <summary>
        /// Dequeue a single event if the event satisfy the predicate
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>The removed event or null if event does not satisfy the predicate or no event available</returns>
        public IEvent DequeueIf(Predicate<IEvent> predicate)
        {
            lock (_queueLock)
            {
                var ev = UnsafePeek();
                if (ev != null && predicate(ev))
                {
                    return UnsafeDequeue();
                }

                return null;
            }
        }

        /// <summary>
        /// Peek into the head of the queue
        /// </summary>
        /// <returns>The event at the head of the queue or null if the queue is empty</returns>
        public IEvent Peek()
        {
            lock (_queueLock)
            {
                return UnsafePeek();
            }
        }

        /// <summary>
        /// Is the queue empty
        /// </summary>
        /// <returns>true if the queue empty</returns>
        public bool IsEmpty()
        {
            lock (_queueLock)
            {
                return _internalQueue.Count == 0;
            }
        }

        /// <summary>
        /// Common method for dequeuing events (not thread safe)
        /// </summary>
        /// <returns>The extracted event or null</returns>
        private IEvent UnsafeDequeue()
        {
            if (_internalQueue.Count == 0)
                return null;

            IEvent removedEvent = _internalQueue.Dequeue();
            TotalQueueEventSize -= removedEvent.EstimatedSize;
            return removedEvent;
        }

        /// <summary>
        /// Peek into the head of the queue (not thread safe)
        /// </summary>
        /// <returns>The event at the head of the queue or null if the queue is empty</returns>
        private IEvent UnsafePeek()
        {
            if (_internalQueue.Count == 0)
                return null;

            IEvent peekedEvent = _internalQueue.Peek();
            return peekedEvent;
        }
    }
}