// <copyright file="AuditdUnitTestBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Security.Tests.Common.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace Agent.Tests.Linux.UnitTests.EventGenerators.AuditdEventGenerators
{
    /// <summary>
    /// Base class for auditd event generators unit tests
    /// </summary>
    [TestClass]
    public abstract class AuditdUnitTestBase : UnitTestBase
    {
        protected AuditEventGeneratorBase _generatorUnderTest;
        protected Mock<IProcessUtil> _mockedShell;

        /// <summary>
        /// Delegate for creating a new instance of derived type.
        /// derived type must be initialized with the given process util
        /// </summary>
        /// <param name="processUtil"></param>
        /// <returns></returns>
        public abstract AuditEventGeneratorBase CreateInstance(IProcessUtil processUtil);

        /// <summary>
        /// Init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
            _mockedShell = new Mock<IProcessUtil>();
            _generatorUnderTest = CreateInstance(_mockedShell.Object);
        }

        /// <summary>
        /// Verifies no events are generated if auditd returns no matches
        /// </summary>
        [TestMethod]
        public void TestNoEventsGeneratedWhenAuditdReturnsNoEvents()
        {
            SetupAusearchReturnValue("");

            var events = _generatorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(0, events.Count());
            _mockedShell.VerifyAll();
        }

        /// <summary>
        /// Test that exception is thrown if auditd is not installed
        /// </summary>
        [TestMethod]
        public void TestExceptionIsThrownIfNoAuditdIsInstalled()
        {
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(cmd => cmd.Contains("auditctl") || cmd.Contains("ausearch")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Throws(new CommandExecutionFailedException("auditctl command", 127, "auditctl: command not found"));

            Assert.ThrowsException<CommandExecutionFailedException>(() =>
            {
                var instance = CreateInstance(_mockedShell.Object);
                instance.GetEvents();
            });
        }

        /// <summary>
        /// The test verifies that ausearch fallback occurs if checkpoint file is corrupted
        /// </summary>
        [TestMethod]
        public void TestAusearchFallbackIsExecuted()
        {
            _mockedShell.SetupSequence(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(cmd => cmd.Contains("ausearch")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Throws(new CommandExecutionFailedException("ausearch cmd", 10, "malformed checkpoint"))
                .Returns("");

            var events = _generatorUnderTest.GetEvents();
            events.ValidateSchema();

            _mockedShell.Verify(m => m.ExecuteProcess(It.IsAny<string>(),
                It.Is<string>(cmd => cmd.Contains("ausearch") && !cmd.Contains("-ts")), It.IsAny<ErrorHandler>(),
                It.IsAny<IEnumerable<int>>()));
            _mockedShell.Verify(m => m.ExecuteProcess(It.IsAny<string>(),
                It.Is<string>(cmd => cmd.Contains("ausearch") && cmd.Contains("-ts")), It.IsAny<ErrorHandler>(),
                It.IsAny<IEnumerable<int>>()));
        }

        protected void SetupAusearchReturnValue(string returnValue)
        {
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(cmd => cmd.Contains("ausearch")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(returnValue);
        }
    }
}