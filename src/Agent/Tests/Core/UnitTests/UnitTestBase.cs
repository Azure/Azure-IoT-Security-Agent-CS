// <copyright file="UnitTestBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.Client;
using Microsoft.Azure.IoT.Agent.Core.Tests.Helpers;
using Microsoft.Azure.IoT.Agent.IoT.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Base class unit tests, initiates the fake twin and client
    /// </summary>
    public class UnitTestBase
    {
        /// <summary>
        /// Helper object for setting twin configurations
        /// </summary>
        protected FakeTwinHelper Twin;

        /// <summary>
        /// Inits the device's twin and the iotHub mocked client.
        /// </summary>
        [TestInitialize]
        public virtual void Init()
        {
            var clientMock = (ModuleClientMock)ModuleClientProvider.GetClient();
            Twin = new FakeTwinHelper(clientMock);
        }
    }
}
