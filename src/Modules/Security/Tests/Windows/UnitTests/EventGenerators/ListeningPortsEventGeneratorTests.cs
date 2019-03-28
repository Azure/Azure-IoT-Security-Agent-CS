// <copyright file="ListeningPortsEventGeneratorTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace Security.Tests.Windows.UnitTests.EventGenerators
{
    /// <summary>
    /// Unit tests for <see cref="ListeningPortsEventGenerator"/>
    /// </summary>
    [TestClass]
    public class ListeningPortsEventGeneratorTests : UnitTestBase
    {
        private Mock<IProcessUtil> _processUtilMock;

        private ListeningPortsEventGenerator _generator;

        /// <summary>
        /// Test initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _processUtilMock = new Mock<IProcessUtil>();
            _processUtilMock.Setup(m => m.ExecuteProcess(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ErrorHandler>(), It.IsAny<IEnumerable<int>>()))
                .Returns("  TCP    0.0.0.0:80             0.0.0.0:0              LISTENING");

            _generator = new ListeningPortsEventGenerator(_processUtilMock.Object);
        }

        /// <summary>
        /// Tests for GetEvents
        /// </summary>
        [TestClass]
        public class GetEventsMethod : ListeningPortsEventGeneratorTests
        {
            /// <summary>
            /// Should use the expected process command
            /// </summary>
            [TestMethod]
            public void ShouldUseExpectedCommand()
            {
                _generator.GetEvents();
                _processUtilMock.Verify(m => m.ExecuteProcess(@"C:\Windows\System32\cmd.exe", "/C \"netstat -an\"", It.IsAny<ErrorHandler>(), It.IsAny<IEnumerable<int>>()), Times.Once);
            }

            /// <summary>
            /// The response from launched process should be parsed and returned as the payload
            /// </summary>
            [TestMethod]
            public void ShouldParseResponseAndReturnTheValuesInTheEventPayload()
            {
                IEnumerable<IEvent> events = _generator.GetEvents();

                Assert.AreEqual(1, events.Count());
                Assert.IsInstanceOfType(events.First(), typeof(ListeningPorts));
                ListeningPorts listeningPortsEvent = (ListeningPorts)events.First();
                Assert.AreEqual(1, listeningPortsEvent.Payload.Count());
                ListeningPortsPayload payload = listeningPortsEvent.Payload.First();

                Assert.AreEqual("80", payload.LocalPort);
                Assert.AreEqual("0", payload.RemotePort);
                Assert.AreEqual("tcp", payload.Protocol);
            }
        }
    }
}
