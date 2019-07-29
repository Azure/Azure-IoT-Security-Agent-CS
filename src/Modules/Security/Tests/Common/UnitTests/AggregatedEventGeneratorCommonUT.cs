// <copyright file="AggregatedEventGeneratorCommonUt.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Security.Tests.Common.UnitTests
{
    /// <summary>
    /// Base class for testing the functionallity of Aggregated event generaotr
    /// </summary>
    [TestClass]
    public abstract class AggregatedEventGeneratorCommonUt<TPayload> : UnitTestBase
        where TPayload : Payload
    {
        /// <summary>
        /// Create a new instance of the generator to test
        /// </summary>
        /// <param name="mockedEventGenerator">mocked event generator to be used as event supplier</param>
        /// <returns></returns>
        protected abstract AggregatedEventsGenerator<TPayload> CreateInstance(EventGenerator mockedEventGenerator);

        /// <summary>
        /// Mocked event1
        /// </summary>
        protected abstract EventBase<TPayload> E1 { get; }

        /// <summary>
        /// Mocked event2
        /// </summary>
        protected abstract EventBase<TPayload> E2 { get; }

        /// <summary>
        /// Change event generator priority
        /// </summary>
        /// <param name="priority">priority to set</param>
        protected abstract void SetEventPriority(EventPriority priority);

        /// <summary>
        /// Enable aggregation
        /// </summary>
        /// <param name="toggle"></param>
        protected abstract void ToggleAggregation(bool toggle);

        /// <summary>
        /// Set aggregation interval
        /// </summary>
        /// <param name="ISO8601Interval">interval to set in ISO8601 format</param>
        protected abstract void SetAggregationIntervalTime(string ISO8601Interval);

        /// <summary>
        /// Compare between two aggregated payloads
        /// </summary>
        /// <param name="p1">payload1</param>
        /// <param name="p2">payload2</param>
        /// <returns>true if p1 equals to p2</returns>
        protected abstract bool ComparePayloads(TPayload p1, TPayload p2);

        private Mock<EventGenerator> _mockedEventGenerator;
        private AggregatedEventsGenerator<TPayload> _generatorUnderTest;

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();

            SetEventPriority(EventPriority.Low);
            _mockedEventGenerator = new Mock<EventGenerator>();
            _generatorUnderTest = CreateInstance(_mockedEventGenerator.Object);
        }

        /// <summary>
        /// Test event generator with aggregation disabled, validated no aggregation
        /// </summary>
        [TestMethod]
        public void TestNoAggregatoin()
        {
            ToggleAggregation(false);
            _mockedEventGenerator
                .Setup(x => x.GetEvents())
                .Returns(new List<IEvent> { E1, E2, E1 });

            var events = _generatorUnderTest.GetEvents();
            Assert.AreEqual(3, events.Count());
            Assert.AreEqual(2, events.Count(x => x == (IEvent) E1));
            Assert.AreEqual(1, events.Count(x => x == (IEvent) E2));
        }

        /// <summary>
        /// Test event generator with aggregation enabled, validated events are aggregated
        /// </summary>
        [TestMethod]
        public void TestAggregatoin()
        {
            ToggleAggregation(true);
            SetAggregationIntervalTime("PT12H");
            _mockedEventGenerator
                .Setup(x => x.GetEvents())
                .Returns(new List<IEvent> { E1, E2, E1 });

            var events = _generatorUnderTest.GetEvents();
            Assert.AreEqual(0, events.Count());
            
            SetAggregationIntervalTime("PT0S");
            events = _generatorUnderTest.GetEvents(); 

            Assert.AreEqual(2, events.Count());
            Assert.IsTrue(events.All(x => x is AggregatedEvent<TPayload>));

            var aggregatedEvents = events.Cast<AggregatedEvent<TPayload>>();
            Assert.IsTrue(aggregatedEvents.Any(e => ComparePayloads(e.Payload.First(), E1.Payload.First())));
            Assert.IsTrue(aggregatedEvents.Any(e => ComparePayloads(e.Payload.First(), E2.Payload.First())));

            var e1Payload = aggregatedEvents.First(e => ComparePayloads(e.Payload.First(), E1.Payload.First())).Payload;
            Assert.AreEqual("4", e1Payload.First().ExtraDetails["HitCount"]);
            var e2Payload = aggregatedEvents.First(e => ComparePayloads(e.Payload.First(), E2.Payload.First())).Payload;
            Assert.AreEqual("2", e2Payload.First().ExtraDetails["HitCount"]);
        }

        /// <summary>
        /// Validates the event generator produces events when enable aggregation is off
        /// and then produces aggregated events when aggregation is on
        /// </summary>
        [TestMethod]
        public void TestAggregatoinTogglingOffToOn()
        {
            ToggleAggregation(false);
            _mockedEventGenerator
                .Setup(x => x.GetEvents())
                .Returns(new List<IEvent> { E1, E2, E1 });

            var events = _generatorUnderTest.GetEvents();
            Assert.AreEqual(3, events.Count());

            ToggleAggregation(true);
            SetAggregationIntervalTime("PT0S");
            events = _generatorUnderTest.GetEvents();

            Assert.AreEqual(2, events.Count());
            Assert.IsTrue(events.All(x => x is AggregatedEvent<TPayload>r));
        }

        /// <summary>
        /// Validates events are aggregated when agrregation is set to on
        /// and then events are not aggregated when aggregation is off
        /// Validate the event generator emits the aggregated event when toggling agrregation to off
        /// </summary>
        [TestMethod]
        public void TestAggregatoinTogglingOnToOff()
        {
            ToggleAggregation(true);
            SetAggregationIntervalTime("PT1H");
            _mockedEventGenerator
                .Setup(x => x.GetEvents())
                .Returns(new List<IEvent> { E1, E2, E1 });

            var events = _generatorUnderTest.GetEvents();
            Assert.AreEqual(0, events.Count());

            ToggleAggregation(false);
            
            events = _generatorUnderTest.GetEvents();

            Assert.AreEqual(5, events.Count());
            Assert.AreEqual(2, events.Count(x => x is AggregatedEvent<TPayload>));
        }
    }
}
