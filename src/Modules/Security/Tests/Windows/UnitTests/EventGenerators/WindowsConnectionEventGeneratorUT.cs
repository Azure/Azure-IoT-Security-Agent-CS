// <copyright file="WindowsConnectionEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Security.Tests.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using static Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils.SystemEventUtils;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.ConnectionsPayload;

namespace Agent.Tests.Windows.UnitTests.EventGenerators
{
    /// <summary>
    /// Unit tests for connections event generator
    /// </summary>
    [TestClass]
    public class WindowsConnectionEventGeneratorUT : UnitTestBase
    {
        private ConnectionEventGenerator _genertorUnderTest;

        private static readonly Dictionary<string, string> InboundConnectionEvent = new Dictionary<string, string>()
        {
            { TimeGeneratedFieldName, DateTime.Now.ToString() },
            { MessageFieldName, @"The Windows Filtering Platform has permitted a connection.

Application Information:
	Process ID:		1500
	Application Name:	\device\harddiskvolume2\windows\system32\svchost.exe

Network Information:
	Direction:		Inbound
	Source Address:		199.195.118.138
	Source Port:		62158
	Destination Address:	10.0.0.7
	Destination Port:		3389
	Protocol:		6

Filter Information:
	Filter Run-Time ID:	66587
	Layer Name:		Receive/Accept
	Layer Run-Time ID:	44" }
        };

        private static readonly Dictionary<string, string> OutboundConnectionEvent = new Dictionary<string, string>()
        {
            { TimeGeneratedFieldName, DateTime.Now.ToString() },
            { MessageFieldName, @"The Windows Filtering Platform has permitted a connection.

Application Information:
	Process ID:		3912
	Application Name:	\device\harddiskvolume2\program files (x86)\internet explorer\iexplore.exe

Network Information:
	Direction:		Outbound
	Source Address:		10.0.0.7
	Source Port:		49890
	Destination Address:	204.79.197.200
	Destination Port:		443
	Protocol:		6

Filter Information:
	Filter Run-Time ID:	66696
	Layer Name:		Connect
	Layer Run-Time ID:	48" }
        };

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init(); 
        }

        /// <summary>
        /// Verifies empty event is created if no rules are configured
        /// </summary>
        [TestMethod]
        public void TestInboundConnectionEvent()
        {
            SetupEvents(InboundConnectionEvent);
            var events = _genertorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(1, events.Count());
            var connectionEvent = (ConnectionCreate)events.First();
            Assert.AreEqual(1, connectionEvent.Payload.Count());
            Assert.AreEqual(ConnectionDirection.In, connectionEvent.Payload.First().Direction);
            Assert.AreEqual(@"\device\harddiskvolume2\windows\system32\svchost.exe", connectionEvent.Payload.First().Executable);
            Assert.AreEqual((uint)1500, connectionEvent.Payload.First().ProcessId);
            Assert.AreEqual("Tcp", connectionEvent.Payload.First().Protocol);
            Assert.AreEqual("199.195.118.138", connectionEvent.Payload.First().RemoteAddress);
            Assert.AreEqual("62158", connectionEvent.Payload.First().RemotePort);
            Assert.AreEqual(null, connectionEvent.Payload.First().UserId);
            Assert.AreEqual(null, connectionEvent.Payload.First().CommandLine);
        }

        /// <summary>
        /// Verifies empty event is created if no rules are configured
        /// </summary>
        [TestMethod]
        public void TestOutboundConnectionEvent()
        {
            SetupEvents(OutboundConnectionEvent);
            var events = _genertorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(1, events.Count());
            var connectionEvent = (ConnectionCreate)events.First();

            Assert.AreEqual(1, connectionEvent.Payload.Count());
            Assert.AreEqual(ConnectionDirection.Out, connectionEvent.Payload.First().Direction);
            Assert.AreEqual(@"\device\harddiskvolume2\program files (x86)\internet explorer\iexplore.exe", connectionEvent.Payload.First().Executable);
            Assert.AreEqual((uint)3912, connectionEvent.Payload.First().ProcessId);
            Assert.AreEqual("Tcp", connectionEvent.Payload.First().Protocol);
            Assert.AreEqual("204.79.197.200", connectionEvent.Payload.First().RemoteAddress);
            Assert.AreEqual("443", connectionEvent.Payload.First().RemotePort);
            Assert.AreEqual(null, connectionEvent.Payload.First().UserId);
            Assert.AreEqual(null, connectionEvent.Payload.First().CommandLine);
        }

        private void SetupEvents(params Dictionary<string, string>[] events)
        {
            var mockedShell = new Mock<IProcessUtil>();
            mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns("");

            var mockedWmi = new Mock<IWmiUtils>();
            mockedWmi.Setup(m => m.RunWmiQuery(
                    It.Is<string>(s=> 
                    s.StartsWith("SELECT Message,TimeGenerated FROM Win32_NTLogEvent Where Logfile = 'Security' AND Eventcode = '5156'")),
                    It.Is<string[]>(a => 
                    a.Contains("Message") && a.Contains("TimeGenerated"))))
                .Returns(events);

            _genertorUnderTest = new ConnectionEventGenerator(mockedShell.Object, mockedWmi.Object);
        }
    }
}