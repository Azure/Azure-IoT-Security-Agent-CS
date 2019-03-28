// <copyright file="DiagnosticEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Security.Tests.Common.UnitTests
{
    /// <summary>
    /// Test class for diagnostic event generator
    /// </summary>
    [TestClass]
    public class DiagnosticEventGeneratorUT : UnitTestBase
    {

        private DiagnosticEventGenerator _genertorUnderTest;

        /// <summary>
        /// Init UT
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _genertorUnderTest = new DiagnosticEventGenerator();
        }

        /// <summary>
        /// Verify no event is sent if there were no diagnostic events
        /// </summary>
        [TestMethod]
        public void TestNoEvents()
        {
            var events = _genertorUnderTest.GetEvents();

            Assert.AreEqual(0, events.Count());
        }

        /// <summary>
        /// Verify events integrity
        /// </summary>
        [TestMethod]
        public void TestEventsAreSentCorrectly()
        {
            var ex = new Exception("Hello");
            string msg1 = "Event1";
            string msg2 = "Event2";

            SystemEvents.DispatchDiagnosicEvent(msg1, LogLevel.Error);
            SystemEvents.DispatchDiagnosicEvent(msg2, LogLevel.Fatal, ex);
            var events = _genertorUnderTest.GetEvents().Cast<Diagnostic>().ToList();

            Assert.AreEqual(2, events.Count());

            var pid = Process.GetCurrentProcess().Id;
            var tid = Thread.CurrentThread.ManagedThreadId;

            var actualEvent = events.ElementAt(0);
            Assert.AreEqual(pid, actualEvent.Payload.First().ProcessId);
            Assert.AreEqual(tid, actualEvent.Payload.First().ThreadId);
            Assert.AreEqual(msg1, actualEvent.Payload.First().Message);
            Assert.AreEqual(LogLevel.Error.ToString(), actualEvent.Payload.First().Severity);
            Assert.AreEqual(ThreadContext.Get().ExecutionId, actualEvent.Payload.First().CorrelationId);

            actualEvent = events.ElementAt(1);
            Assert.AreEqual(pid, actualEvent.Payload.First().ProcessId);
            Assert.AreEqual(tid, actualEvent.Payload.First().ThreadId);
            Assert.AreEqual(ex.FormatExceptionMessage(msg2), actualEvent.Payload.First().Message);
            Assert.AreEqual(LogLevel.Fatal.ToString(), actualEvent.Payload.First().Severity);
            Assert.AreEqual(ThreadContext.Get().ExecutionId, actualEvent.Payload.First().CorrelationId);
        }

        /// <summary>
        /// Verify queue is cleared after each "GetEvents" call
        /// </summary>
        [TestMethod]
        public void TestQueueIsClearedAfterGetEvents()
        {
            var ex = new Exception("Hello");
            string msg1 = "Event1";
            string msg2 = "Event2";

            SystemEvents.DispatchDiagnosicEvent(msg1, LogLevel.Error);
            SystemEvents.DispatchDiagnosicEvent(msg2, LogLevel.Fatal, ex);
            var events = _genertorUnderTest.GetEvents().Cast<Diagnostic>();

            Assert.AreEqual(2, events.Count());

            events = _genertorUnderTest.GetEvents().Cast<Diagnostic>();
            Assert.AreEqual(0, events.Count());
        }

        /// <summary>
        /// Verify queue does not inflate over it's limit
        /// </summary>
        [TestMethod]
        public void TestQueueSizeDoesNotPassLimit()
        {
            var internalQueueSize = (uint)(AgentConfiguration.RemoteConfiguration.MaxLocalCacheSize * 0.2);

            string msg1 = "Event1";
            SystemEvents.DispatchDiagnosicEvent(msg1, LogLevel.Error);
            var events = _genertorUnderTest.GetEvents().Cast<Diagnostic>();
            var eventSize = events.First().EstimatedSize;

            uint totalSize = 0;
            while (totalSize < 6 * internalQueueSize)
            {
                SystemEvents.DispatchDiagnosicEvent(msg1, LogLevel.Error);
                totalSize += eventSize;
            }

            events = _genertorUnderTest.GetEvents().Cast<Diagnostic>();

            uint actualSize = 0;
            foreach (var ev in events)
            {
                actualSize += ev.EstimatedSize;
            }

            Assert.IsTrue(actualSize <= internalQueueSize);
        }
    }
}