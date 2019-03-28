// <copyright file="EventQueueManagerTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.AgentTelemetry;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="EventQueueManager"/>
    /// </summary>
    [TestClass]
    public class EventQueueManagerTests
    {
        private EventQueueManager _queueManager;

        /// <summary>
        /// Test initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            EventQueueManager.ResetInstance();
            _queueManager = EventQueueManager.Instance;
        }

        /// <summary>
        /// Tests for the <see cref="EventQueueManager.ResetInstance"/> method
        /// </summary>
        [TestClass]
        public class ResetInstanceMethod : EventQueueManagerTests
        {
            /// <summary>
            /// <see cref="EventQueueManager.EventDroppedHandlers"/> should be unregistered
            /// </summary>
            [TestMethod]
            public void ShouldUnregisterToEventDroppedEvent()
            {
                EventQueueManager.Instance.GetType(); // trigger lazy initialization
                EventQueueManager.OnEventDropped(EventPriority.High, 1);
                EventQueueManager.ResetInstance();
                EventQueueManager.Instance.GetType(); // trigger lazy initialization
                EventQueueManager.OnEventDropped(EventPriority.High, 1);
                Assert.AreEqual(2, TelemetryCollector.GetDataAndReset()[CounterType.DroppedHighPriorityEvent]);
            }
            /// <summary>
            /// <see cref="EventQueueManager.EventEnqueuedHandlers"/> should be unregistered
            /// </summary>
            [TestMethod]
            public void ShouldUnregisterToEventEnqueuedEvent()
            {
                EventQueueManager.Instance.GetType(); // trigger lazy initialization
                EventQueueManager.OnEventEnqueued(EventPriority.High, 1);
                EventQueueManager.ResetInstance();
                EventQueueManager.Instance.GetType(); // trigger lazy initialization
                EventQueueManager.OnEventEnqueued(EventPriority.High, 1);
                Assert.AreEqual(2, TelemetryCollector.GetDataAndReset()[CounterType.EnqueuedHighPriorityEvent]);
            }
        }
    }
}
