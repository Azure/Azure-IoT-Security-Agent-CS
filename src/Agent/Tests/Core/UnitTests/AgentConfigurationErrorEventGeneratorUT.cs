// <copyright file="AgentConfigurationErrorEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Linq;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Test class for agent configuration error event generator
    /// </summary>
    [TestClass]
    public class AgentConfigurationErrorEventGeneratorUT : UnitTestBase
    {
        private AgentConfigurationErrorEventGenerator _genertorUnderTest;

        /// <summary>
        /// Init UT
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _genertorUnderTest = new AgentConfigurationErrorEventGenerator();
            Twin.ChangeConfiguration("maxMessageSizeInBytes", "8192")
                .ChangeConfiguration("maxLocalCacheSizeInBytes", "8192")
                .ChangeConfiguration("lowPriorityMessageFrequency", "PT10S")
                .ChangeConfiguration("highPriorityMessageFrequency", "PT10S")
                .UpdateAgentTwinConfiguration();

            _genertorUnderTest.GetEvents();
        }

        /// <summary>
        /// Test configuration error operational event is not fired if configuration is ok
        /// </summary>
        [TestMethod]
        public void TestNoConfigurationErrorEventsIfConfigIsOk()
        {
            Twin.ChangeConfiguration("maxMessageSizeInBytes", "4096");
            var events = _genertorUnderTest.GetEvents();
            Assert.AreEqual(0, events.Count());
        }

        /// <summary>
        /// Test configuration error operational is fired for not optimal message size
        /// </summary>
        [TestMethod]
        public void TestMessageSizeNotOptimal()
        {
            Twin.ChangeConfiguration("maxMessageSizeInBytes", "8000");

            var events = _genertorUnderTest.GetEvents();
            Assert.AreEqual(1, events.Count());

            var ev = events.Cast<ConfigurationError>().First();
            Assert.AreEqual(1, ev.Payload.Count());

            var payload = ev.Payload.First();

            Assert.AreEqual("maxMessageSizeInBytes", payload.ConfigurationName);
            Assert.AreEqual(ConfigurationErrorType.NotOptimal, payload.ErrorType);
            Assert.AreEqual("8000", payload.UsedConfiguration);
        }

        /// <summary>
        /// Test configuration error operational is fired for not optimal cache size
        /// </summary>
        [TestMethod]
        public void TestCacheSizeIsSmallerThanMessageSize()
        {
            Twin.ChangeConfiguration("maxLocalCacheSizeInBytes", "8000");

            var events = _genertorUnderTest.GetEvents();
            Assert.AreEqual(1, events.Count());

            var ev = events.Cast<ConfigurationError>().First();
            Assert.AreEqual(1, ev.Payload.Count());

            var payload = ev.Payload.First();

            Assert.AreEqual("maxLocalCacheSizeInBytes", payload.ConfigurationName);
            Assert.AreEqual(ConfigurationErrorType.Conflict, payload.ErrorType);
            Assert.AreEqual("8000", payload.UsedConfiguration);
        }

        /// <summary>
        /// Test configuration error operational is fired for conflict with high and low priority intervals
        /// </summary>
        [TestMethod]
        public void TestHighPrioIntervalGreaterThanLowPrioInterval()
        {
            var oldTwin = AgentConfiguration.RemoteConfiguration;
            Twin.ChangeConfiguration("highPriorityMessageFrequency", "PT11H11M11S");

            var events = _genertorUnderTest.GetEvents();
            Assert.AreEqual(1, events.Count());

            var ev = events.Cast<ConfigurationError>().First();
            Assert.AreEqual(1, ev.Payload.Count());

            var payload = ev.Payload.First();

            Assert.AreEqual("highPriorityMessageFrequency", payload.ConfigurationName);
            Assert.AreEqual(ConfigurationErrorType.Conflict, payload.ErrorType);
            Assert.AreEqual("11:11:11", payload.UsedConfiguration);
        }

        /// <summary>
        /// Test configuration error operational is fired for type mismatch
        /// </summary>
        [TestMethod]
        public void TestBadTypeConfiguration()
        {
            var oldTwin = AgentConfiguration.RemoteConfiguration;
            Twin.ChangeConfiguration("highPriorityMessageFrequency", "I'm a string, not a TimeSpan");

            var events = _genertorUnderTest.GetEvents();
            Assert.AreEqual(1, events.Count());

            var ev = events.Cast<ConfigurationError>().First();
            Assert.AreEqual(1, ev.Payload.Count());

            var payload = ev.Payload.First();

            Assert.AreEqual(nameof(RemoteConfiguration), payload.ConfigurationName);
            Assert.AreEqual(ConfigurationErrorType.TypeMismatch, payload.ErrorType);
            Assert.AreEqual(oldTwin, AgentConfiguration.RemoteConfiguration);
        }
    }
}
