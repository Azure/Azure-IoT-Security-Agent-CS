// <copyright file="OperationalEventsNonScheduledTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.Providers;
using Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents;
using Microsoft.Azure.IoT.Agent.Core.Tests.Helpers;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Tests
{
    /// <summary>
    /// Integration tests for device agent.
    /// The tests below does not use the scheduler
    /// </summary>
    [TestClass]
    public class OperationalEventsNonScheduledTests : IntegrationTestBase
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
        /// Verifies operational event are not fired if a security messages is not fired
        /// </summary>
        [TestMethod]
        public void TestOperationalEventIsNotFiredIfAMessageIsNotFired()
        {
            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.Operational)
                .ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.Off)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.Off)
                .UpdateAgentTwinConfiguration();

            var expectedOperationalEvent = FakesEventsFactory.CreateFakeOperationalEvent();
            var expectedPeriodicEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
            var expectedTriggeredEvent = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.Low);

            FakeOperationalEventGenerator.SetEvents(new[] { expectedOperationalEvent });
            FakeSnapshotEventGenerator.SetEvents(new[] { expectedPeriodicEvent });
            FakeTriggeredEventGenerator.SetEvents(new[] { expectedTriggeredEvent });

            _eventProducer.ExecuteTask();
            _eventProducer.ExecuteTask();

            _messageBuilderConsumer.ExecuteTask();

            var sentMessages = ClientMock.GetMessages();
            Assert.AreEqual(0, sentMessages.Count);
        }

        /// <summary>
        /// Verifies operational events are fired with the first security message
        /// </summary>
        [TestMethod]
        public void TestOperationalEventsAreFiredWithFirestMessage()
        {
            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.Operational)
                .ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.Low)
                .UpdateAgentTwinConfiguration();

            var expectedOperationalEvent = FakesEventsFactory.CreateFakeOperationalEvent();
            var expectedPeriodicEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
            var expectedTriggeredEvent = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.Low);

            FakeOperationalEventGenerator.SetEvents(new[] { expectedOperationalEvent });
            FakeSnapshotEventGenerator.SetEvents(new[] { expectedPeriodicEvent });
            FakeTriggeredEventGenerator.SetEvents(new[] { expectedTriggeredEvent });

            _eventProducer.ExecuteTask();
            _eventProducer.ExecuteTask();

            _messageBuilderConsumer.ExecuteTask();

            var sentMessages = ClientMock.GetMessages();
            Assert.AreEqual(2, sentMessages.Count);

            var firstMsg = MessageHelper.GetEventsFromMsg(sentMessages[0].GetBytes());
            Assert.AreEqual(2, firstMsg.Count(ev => ev.EventType == EventType.Operational));

            var secondMessage = MessageHelper.GetEventsFromMsg(sentMessages[1].GetBytes());
            Assert.AreEqual(0, secondMessage.Count(ev => ev.EventType == EventType.Operational));
        }

        /// <summary>
        /// Verifies message with operational events does not exceeds max message size
        /// </summary>
        [TestMethod]
        public void TestMessageWithOperationalEventDoesNotExceedMessageLimit()
        {
            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.Operational)
                .ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.High)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.Low)
                .UpdateAgentTwinConfiguration();

            var expectedOperationalEvent = FakesEventsFactory.CreateFakeOperationalEvent();
            var expectedPeriodicEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
            var expectedTriggeredEvent = FakesEventsFactory.CreateFakeTriggeredEvent(EventPriority.Low);

            FakeOperationalEventGenerator.SetEvents(new[] { expectedOperationalEvent });
            FakeSnapshotEventGenerator.SetEvents(new[] { expectedPeriodicEvent });
            FakeTriggeredEventGenerator.SetEvents(new[] { expectedTriggeredEvent });

            for (int i = 0; i < 10; i++)
            {
                _eventProducer.ExecuteTask();
            }
            _messageBuilderConsumer.ExecuteTask();

            Assert.IsTrue(MessageHelper.WaitUntilAsyncMessageSendsComplete(ClientMock));

            var sentMessages = ClientMock.GetMessages();
            Assert.AreEqual(3, sentMessages.Count);

            foreach (var msg in sentMessages)
            {
                Assert.IsTrue(msg.BodyStream.Length < AgentConfiguration.RemoteConfiguration.MaxMessageSize);
            }
        }
    }
}
