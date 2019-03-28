// <copyright file="NonScheduledIntegrationTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.Providers;
using Microsoft.Azure.IoT.Agent.Core.Scheduling;
using Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents;
using Microsoft.Azure.IoT.Agent.Core.Tests.Helpers;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Tests
{
    /// <summary>
    /// Integration tests for device agent.
    /// The tests below does not use the scheduler
    /// </summary>
    [TestClass]
    public class NonScheduledIntegrationTests : IntegrationTestBase
    {
        private EventProducer _eventProducer;
        private MessageBuilder _messageBuilderConsumer;

        /// <summary>
        /// Test initialization method
        /// This method occurs before each test method and assures clean start
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();

            EventQueueManager.ResetInstance();
            _eventProducer = new EventProducer(new AppConfigEventGeneratorsProvider().GetAll().ToArray());
            _messageBuilderConsumer = new MessageBuilder();
        }

        /// <summary>
        /// Verifies high priority event is sent correctly
        /// </summary>
        [TestMethod]
        public void TestHighPriorityEventFromSingleGenerator()
        {
            Twin.ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.High)
                .UpdateAgentTwinConfiguration();

            var expectedEvents = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.High);
            FakeTriggeredEventGenerator.SetEvents(new[] {expectedEvents});

            TestSingleEvent(expectedEvents);
        }

        /// <summary>
        /// Verifies low priority event is sent correctly
        /// </summary>
        [TestMethod]
        public void TestLowPrioEventFromSingleGenerator()
        {
            Twin.ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.Low)
                .UpdateAgentTwinConfiguration();

            var expectedEvents = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.High);
            FakeTriggeredEventGenerator.SetEvents(new[] { expectedEvents });

            TestSingleEvent(expectedEvents);
        }

        /// <summary>
        /// Verifies no events are sent if all generators priorities are off
        /// </summary>
        [TestMethod]
        public void TestNoEventsAreFiredIfAllGeneratorsPrioirtyIsOff()
        {
            _eventProducer.ExecuteTask();
            _messageBuilderConsumer.ExecuteTask();

            var sentMessages = ClientMock.GetMessages();

            Assert.AreEqual(0, sentMessages.Count);
        }

        /// <summary>
        /// Verifies that events are not sent if event size are too big (bigger than message size)
        /// </summary>
        //TODO: Once operational events will be active, need to check that an operational event is fired letting us know that a message was dropped
        [TestMethod]
        public void TestEventNotFiredIfEventIsTooBig()
        {
            var maxMessageSize = Int32.Parse(Twin.GetConfiguration("maxMessageSizeInBytes"));
            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.High)
    .UpdateAgentTwinConfiguration();
            Assert.AreEqual((uint) maxMessageSize, AgentConfiguration.RemoteConfiguration.MaxMessageSize);

            var expectedEvent = FakesEventsFactory.CreateFakeOperationalEvent(GenerateBigFakePayload(maxMessageSize));
            FakeOperationalEventGenerator.SetEvents(new[] {expectedEvent});

            RepeatAction(_eventProducer, 100);

            _messageBuilderConsumer.ExecuteTask();
            var sentMessages = ClientMock.GetMessages();

            Assert.AreEqual(0, sentMessages.Count);
        }

        /// <summary>
        /// Test that message size does not exceeds messageSize limit
        /// </summary>
        [TestMethod]
        public void TestMessageSizeDoesNotExceedsLimit()
        {
            var maxMessageSize = Int32.Parse(Twin.GetConfiguration("maxMessageSizeInBytes"));
            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.High)
                .UpdateAgentTwinConfiguration();

            var expectedOperationalEvent = FakesEventsFactory.CreateFakeOperationalEvent();
            var expectedPeriodicEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
            var expectedTriggeredEvent = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.High);

            FakeOperationalEventGenerator.SetEvents(new[] { expectedOperationalEvent });
            FakeSnapshotEventGenerator.SetEvents(new[] { expectedPeriodicEvent });
            FakeTriggeredEventGenerator.SetEvents(new[] { expectedTriggeredEvent });

            RepeatAction(_eventProducer, 100);
            RepeatAction(_messageBuilderConsumer, 100);

            Assert.IsTrue(MessageHelper.WaitUntilAsyncMessageSendsComplete(ClientMock));

            var sentMessages = ClientMock.GetMessages();
            Assert.IsTrue(sentMessages.Count > 0);

            foreach (var msg in sentMessages)
            {
                Assert.IsTrue(msg.GetBytes().Length < maxMessageSize);
            }
        }

        /// <summary>
        /// Verifies multiple events with different priorities are sent correctly
        /// </summary>
        [TestMethod]
        public void TestEventAreFiredCorrectlyFromMultipleGenerators()
        {
            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.Low)
                .UpdateAgentTwinConfiguration();

            var expectedOperationalEvent = FakesEventsFactory.CreateFakeOperationalEvent();
            var expectedPeriodicEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
            var expectedTriggeredEvent = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.Low);

            FakeOperationalEventGenerator.SetEvents(new []{ expectedOperationalEvent });
            FakeSnapshotEventGenerator.SetEvents(new [] { expectedPeriodicEvent });
            FakeTriggeredEventGenerator.SetEvents(new [] { expectedTriggeredEvent });

            _eventProducer.ExecuteTask();
            _eventProducer.ExecuteTask();

            _messageBuilderConsumer.ExecuteTask();

            var sentMessages = ClientMock.GetMessages();
            Assert.AreEqual(2, sentMessages.Count);

            var sentEvents = new List<Event>();
            foreach (var msg in sentMessages)
            {
                byte[] msgBytes = msg.GetBytes();
                MessageHelper.VerifyMessageHeader(msgBytes);
                sentEvents.AddRange(MessageHelper.GetEventsFromMsg(msgBytes));
            }

            foreach (var ev in sentEvents)
            {
                if (ev.Name == typeof(FakePeriodicEvent).Name)
                {
                    MessageHelper.ValidateEventsAreEqual(ev, expectedPeriodicEvent);
                }
                else if (ev.Name == typeof(FakeTriggeredEvent).Name)
                {
                    MessageHelper.ValidateEventsAreEqual(ev, expectedTriggeredEvent);
                }
                else if (ev.Name == typeof(FakeOperationalEvent).Name)
                {
                    MessageHelper.ValidateEventsAreEqual(ev, expectedOperationalEvent);
                }
            }
        }

        /// <summary>
        /// Verifies that piggyback mechanism
        /// If all high priority messages are dequeued and maxMessageSize -messageSize > 0
        /// dequeue low priority events and send the in the same message
        /// </summary>
        [TestMethod]
        public void TestMessagePiggybacking()
        {
            int maxMessageSize = 3000;
            Twin.ChangeConfiguration("maxMessageSizeInBytes", maxMessageSize.ToString())
                .ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.Low)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.Low)
                .UpdateAgentTwinConfiguration();

            var expectedOperationalEvent = FakesEventsFactory.CreateFakeOperationalEvent();
            var expectedPeriodicEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.Low);
            var expectedTriggeredEvent = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.Low);

            FakeOperationalEventGenerator.SetEvents(new[] { expectedOperationalEvent });
            FakeSnapshotEventGenerator.SetEvents(new[] { expectedPeriodicEvent });
            FakeTriggeredEventGenerator.SetEvents(new[] { expectedTriggeredEvent });

            _eventProducer.ExecuteTask();
            _messageBuilderConsumer.ExecuteTask();
            
            //One message should fit for all 3 events
            var sentMessages = ClientMock.GetMessages();
            Assert.AreEqual(1, sentMessages.Count);
            var events = MessageHelper.GetEventsFromMsg(sentMessages.First().GetBytes());

            Assert.AreEqual(1, events.Count(ev => ev.Name == typeof(FakeOperationalEvent).Name));
            Assert.AreEqual(1, events.Count(ev => ev.Name == typeof(FakePeriodicEvent).Name));
            Assert.AreEqual(1, events.Count(ev => ev.Name == typeof(FakeTriggeredEvent).Name));
        }

        /// <summary>
        /// Runs the producer and the consumer and verify that the event sent is equal to expected event
        /// </summary>
        /// <param name="expectedEvent"></param>
        private void TestSingleEvent(EventBase<FakeEventPayload> expectedEvent)
        {
            _eventProducer.ExecuteTask();
            _messageBuilderConsumer.ExecuteTask();

            var sentMessages = ClientMock.GetMessages();

            Assert.AreEqual(1, sentMessages.Count);

            byte[] msgBytes = sentMessages.First().GetBytes();
            MessageHelper.VerifyMessageHeader(msgBytes);
            var actuaclEvent = MessageHelper.GetEventsFromMsg(msgBytes).First();
            MessageHelper.ValidateEventsAreEqual(actuaclEvent, expectedEvent);
        }

        /// <summary>
        /// Repeats an ITask action for the given amount of times
        /// </summary>
        /// <param name="task"></param>
        /// <param name="times"></param>
        private void RepeatAction(ITask task, int times)
        {
            for (int i = 0; i < times; i++)
            {
                task.ExecuteTask();
            }
        }

        /// <summary>
        /// Generates FakeEventPayload with size > sizeAtLeast
        /// </summary>
        /// <param name="sizeAtLeast"></param>
        /// <returns></returns>
        private FakeEventPayload GenerateBigFakePayload(int sizeAtLeast)
        {
            return new FakeEventPayload
            {
                Param1 = new String('a', sizeAtLeast),
                Param2 = string.Empty
            };
        }
    }
}