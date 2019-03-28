// <copyright file="IntegrationTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents;
using Microsoft.Azure.IoT.Agent.Core.Tests.Helpers;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.IoT.Agent.Core.Tests
{
    /// <summary>
    /// Integration Tests for Agent interfaces from event generators to sent messages
    /// </summary>
    [TestClass]
    public class IntegrationTests : IntegrationTestBase
    {
        /// <summary>
        /// This Test verifies that if a twin configuration has changed, the Agent twin gets updated as well
        /// </summary>
        [TestMethod]
        public void TesTDeviceTwinConfigurationIsChangedIfTwinIsChanged()
        {
            NameValueCollection generalConfig = (NameValueCollection)ConfigurationManager.GetSection("General");

            var generatorUnderTest = new FakeOperationalEventGenerator();
            Assert.AreEqual(EventPriority.Off, generatorUnderTest.Priority);

            Twin.ChangeEventPriority(typeof(FakeOperationalEvent).Name, EventPriority.High);


            Assert.AreEqual(EventPriority.High, generatorUnderTest.Priority);
        }

        /// <summary>
        /// This test verifies snapshot events occurs at snapshot intervals
        /// </summary>
        [TestMethod]
        public void TestSnapshotEventsIsFiredOnSnapshotInerval()
        {
            var snapshotInterval = TimeSpan.FromSeconds(10);

            Twin.ChangeEventPriority(typeof(FakePeriodicEvent).Name, EventPriority.Low)
                .ChangeEventPriority(typeof(FakeTriggeredEvent).Name, EventPriority.High)
                .ChangeConfiguration("snapshotFrequency", XmlConvert.ToString(snapshotInterval))
                .UpdateAgentTwinConfiguration();

            Assert.AreEqual(snapshotInterval, AgentConfiguration.RemoteConfiguration.SnapshotFrequency);

            using (SingleThreadedAgent agent = new SingleThreadedAgent())
            {
                var taskUnderTest = new Task(agent.Start);
                taskUnderTest.Start();

                var expectedEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
                FakeSnapshotEventGenerator.SetEvents(new[] { expectedEvent });

                while (ClientMock.GetMessages().Count == 0) ;
                VerifySnapshot(ClientMock.GetMessages(), new[] { expectedEvent });
                ClientMock.Reset();
                var expectedSecondEvent = FakesEventsFactory.CreateFakePeriodicEvent(EventPriority.High);
                FakeSnapshotEventGenerator.SetEvents(new[] { expectedSecondEvent });

                Thread.Sleep(snapshotInterval + AgentConfiguration.RemoteConfiguration.HighPriorityMessageFrequency);

                VerifySnapshot(ClientMock.GetMessages(), new[] {expectedSecondEvent});
            }
        }

        private void VerifySnapshot(IList<Message> msg, IList<FakePeriodicEvent> expectedEvents)
        {
            var events = msg.Select(m => m.GetBytes()).Select(MessageHelper.GetEventsFromMsg).SelectMany(ev => ev);
            var snapshotEvents = events.Where(ev => ev.Name == typeof(FakePeriodicEvent).Name).ToList();

            Assert.AreEqual(expectedEvents.Count, snapshotEvents.Count);

            for (int i = 0; i < expectedEvents.Count; i++)
            {
                MessageHelper.ValidateEventsAreEqual(snapshotEvents.ElementAt(i), expectedEvents.ElementAt(i));
            }
        }
    }
}