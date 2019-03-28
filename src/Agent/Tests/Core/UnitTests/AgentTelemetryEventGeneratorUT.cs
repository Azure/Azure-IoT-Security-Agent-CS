// <copyright file="AgentTelemetryEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.AgentTelemetry;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Test class for diagnostic event generator
    /// </summary>
    [TestClass]
    public class AgentTelemetryEventGeneratorUT : UnitTestBase
    {

        private AgentTelemetryEventGenerator _genertorUnderTest;

        private const int smallMsg = 1;
        private const int failedMsg = 2;
        private const int sentMsg = 3;

        private const int droppedHigh = 4;
        private const int enqueuedHigh = 5;
        private const int droppedLow = 6;
        private const int enqueuedLow = 7;


        /// <summary>
        /// Init UT
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _genertorUnderTest = new AgentTelemetryEventGenerator();
        }

        /// <summary>
        /// Verify events integrity
        /// </summary>
        [TestMethod]
        public void TestMetricsAreCollectedCorrectly()
        {
            SetCollectors();
            var actualEvent = _genertorUnderTest.GetEvents();

            Assert.AreEqual(2, actualEvent.Count());

            var msgStat = (MessageStatistics) actualEvent.First(ev => ev.Name == typeof(MessageStatistics).Name);
            var droppedEventStat = (DroppedEventsStatistics) actualEvent.First(ev => ev.Name == typeof(DroppedEventsStatistics).Name);

            Assert.AreEqual(smallMsg, msgStat.Payload.First().MessagesUnder4KB);
            Assert.AreEqual(failedMsg, msgStat.Payload.First().TotalFailed);
            Assert.AreEqual(sentMsg + failedMsg, msgStat.Payload.First().MessagesSent);

            Assert.AreEqual(3, droppedEventStat.Payload.Count());

            Assert.AreEqual(1, droppedEventStat.Payload.Count(p => p.DroppedEvents == droppedHigh && p.Queue == EventPriority.High));
            Assert.AreEqual(1, droppedEventStat.Payload.Count(p => p.CollectedEvents == enqueuedHigh && p.Queue == EventPriority.High));
            Assert.AreEqual(1, droppedEventStat.Payload.Count(p => p.DroppedEvents == droppedLow && p.Queue == EventPriority.Low));
            Assert.AreEqual(1, droppedEventStat.Payload.Count(p => p.CollectedEvents == enqueuedLow && p.Queue == EventPriority.Low));
        }

        /// <summary>
        /// Verify queue is cleared after each "GetEvents" call
        /// </summary>
        [TestMethod]
        public void TestCollectorsResetOnGetData()
        {
            SetCollectors();

            var data = TelemetryCollector.GetDataAndReset();
            data = TelemetryCollector.GetDataAndReset();

            Assert.IsFalse(data.Any(x => x.Value != 0));
        }

        private void SetCollectors()
        {
            TelemetryCollector.GetDataAndReset();
            CounterType.MessagesUnder4KB.Get().IncrementBy(smallMsg);
            CounterType.SendSuccesfully.Get().IncrementBy(sentMsg);
            CounterType.SendFailed.Get().IncrementBy(failedMsg);

            CounterType.DroppedHighPriorityEvent.Get().IncrementBy(droppedHigh);
            CounterType.EnqueuedHighPriorityEvent.Get().IncrementBy(enqueuedHigh);

            CounterType.DroppedLowPriorityEvent.Get().IncrementBy(droppedLow);
            CounterType.EnqueuedLowPriorityEvent.Get().IncrementBy(enqueuedLow);
        }
    }
}