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

        private void MockBaselineOutput(string output)
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

        /// <summary>
        /// Verify normal (happy) flow
        /// </summary>
        [TestMethod]
        public void TestBaselineOutputParsedSuccessfully()
        {
            MockBaselineOutput(Resources.baseline_output_nice);

            var events = _genertorUnderTest.GetEvents().ToList();

            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(8, ((OSBaseline)events.First()).Payload.Count());
        }

        /// <summary>
        /// Verify normal (happy) flow
        /// </summary>
        [TestMethod]
        public void TestBaselineCustomChecksPopulation()
        {
            MockBaselineOutput(Resources.baseline_output_nice);

            Twin.ChangeConfiguration("baselineCustomChecksFilePath", "/file/path")
                .ChangeConfiguration("baselineCustomChecksFileHash", "file_hash#")
                .ChangeConfiguration("baselineCustomChecksEnabled", true.ToString());

            var events = _genertorUnderTest.GetEvents().ToList();

            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(16, ((OSBaseline)events.First()).Payload.Count());
        }

        /// <summary>
        /// Test that all requirements are met to allow omsbaseline custom checks execution
        /// </summary>
        [DataTestMethod]
        [DataRow("/file/path", "file_hash#", true)]
        public void TestIsCustomChecksEnabledPositive(string baselineCustomChecksFilePath, string baselineCustomChecksFileHash, bool baselineCustomChecksEnabled)
        {
            Twin.ChangeConfiguration("baselineCustomChecksFilePath", baselineCustomChecksFilePath)
                .ChangeConfiguration("baselineCustomChecksFileHash", baselineCustomChecksFileHash)
                .ChangeConfiguration("baselineCustomChecksEnabled", baselineCustomChecksEnabled.ToString());

            Assert.IsTrue(BaselineEventGenerator.IsCustomChecksEnabled());
        }

        /// <summary>
        /// Test that if not all requirements are met omsbaseline custom checks execution is disabled
        /// </summary>
        [DataTestMethod]
        [DataRow(null, null, true)]
        [DataRow(null, "file_hash#", true)]
        [DataRow("/file/path", null, true)]
        [DataRow(null, null, false)]
        [DataRow("/file/path", "file_hash#", false)]
        [DataRow("/file/path", null, false)]
        [DataRow(null, "file_hash#", false)]
        public void TestIsCustomChecksEnabledNegative(string baselineCustomChecksFilePath, string baselineCustomChecksFileHash, bool baselineCustomChecksEnabled)
        {
            Twin.ChangeConfiguration("baselineCustomChecksFilePath", baselineCustomChecksFilePath)
                .ChangeConfiguration("baselineCustomChecksFileHash", baselineCustomChecksFileHash)
                .ChangeConfiguration("baselineCustomChecksEnabled", baselineCustomChecksEnabled.ToString());

            Assert.IsFalse(BaselineEventGenerator.IsCustomChecksEnabled());
        }

        /// <summary>
        /// Test that all requirements are met to allow omsbaseline custom checks execution
        /// </summary>
        [TestMethod]
        public void TestIsCustomChecksEnabledPositive()
        {
            Twin.ChangeConfiguration("baselineCustomChecksFilePath", "/file/path")
                .ChangeConfiguration("baselineCustomChecksFileHash", "file_hash#")
                .ChangeConfiguration("baselineCustomChecksEnabled", "true");

            Assert.IsTrue(BaselineEventGenerator.IsCustomChecksEnabled());
        }

        /// <summary>
        /// Test that if not all requirements are met omsbaseline custom checks execution is disabled
        /// </summary>
        [TestMethod]
        public void TestIsCustomChecksEnabledNegative()
        {
            Twin.ChangeConfiguration("baselineCustomChecksFilePath", null)
                .ChangeConfiguration("baselineCustomChecksFileHash", null)
                .ChangeConfiguration("baselineCustomChecksEnabled", "false");
            Assert.IsFalse(BaselineEventGenerator.IsCustomChecksEnabled());

            Twin.ChangeConfiguration("baselineCustomChecksEnabled", "true");
            Assert.IsFalse(BaselineEventGenerator.IsCustomChecksEnabled());

            Twin.ChangeConfiguration("baselineCustomChecksEnabled", "false");
            Twin.ChangeConfiguration("baselineCustomChecksFilePath", "not-null-or-empty");
            Assert.IsFalse(BaselineEventGenerator.IsCustomChecksEnabled());
        }
    }
}
