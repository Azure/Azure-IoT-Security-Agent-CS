// <copyright file="BaselineEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Security.Tests.Linux.Properties;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Linux.UnitTests.EventGenerators
{
    /// <summary>
    /// Unit tests for baseline event generator
    /// </summary>
    [TestClass]
    public class BaselineEventGeneratorUT : UnitTestBase
    {
        private BaselineEventGenerator _genertorUnderTest;

        private Mock<IProcessUtil> _mockedShell;

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Verify normal (happy) flow
        /// </summary>
        [TestMethod]
        public void BaselineOutputIsParsedWithoutError()
        {
            SetupBaselineOutput(Resources.baseline_output_nice);

            var events = _genertorUnderTest.GetEvents().ToList();

            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(8, ((OSBaseline)events.First()).Payload.Count());
        }

        private void SetupBaselineOutput(string output)
        {
            _mockedShell = new Mock<IProcessUtil>();
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(output);

            _genertorUnderTest = new BaselineEventGenerator(_mockedShell.Object);
        }
    }
}
