// <copyright file="InboundConnEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Agent.Tests.Linux.UnitTests.EventGenerators.AuditdEventGenerators
{
    /// <summary>
    /// Unit tests for inbound connection event generator
    /// </summary>
    [TestClass]
    public class InboundConnEventGeneratorUT : AuditdUnitTestBase
    {
        /// <inheritdoc />
        public override AuditEventGeneratorBase CreateInstance(IProcessUtil processUtil)
        {
            return new InboundConnEventGenerator(processUtil);
        }
    }
}
