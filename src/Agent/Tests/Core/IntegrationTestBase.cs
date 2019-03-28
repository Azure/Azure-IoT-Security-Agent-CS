// <copyright file="IntegrationTestBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.Client;
using Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents;
using Microsoft.Azure.IoT.Agent.Core.Tests.Helpers;
using Microsoft.Azure.IoT.Agent.Core.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Azure.IoT.Agent.IoT.Providers;

namespace Microsoft.Azure.IoT.Agent.Core.Tests
{
    /// <summary>
    /// Base class for integration testings, used for initialization
    /// </summary>
    [TestClass]
    public class IntegrationTestBase
    {
        /// <summary>
        /// Mock of ModuleClient
        /// This client is responsible for sending message and getting the device twin
        /// </summary>
        protected ModuleClientMock ClientMock;

        /// <summary>
        /// Helper object for setting twin configurations
        /// </summary>
        protected FakeTwinHelper Twin;

        /// <summary>
        /// fake events factory
        /// </summary>
        protected FakeEventsFactory FakesEventsFactory;

        /// <summary>
        /// Inits the device's twin and the iotHub mocked client.
        /// </summary>
        [TestInitialize]
        public virtual void Init()
        {
            //Reset all fake event generators to produce empty events
            FakeOperationalEventGenerator.SetEvents(new List<FakeOperationalEvent>());
            FakeSnapshotEventGenerator.SetEvents(new List<FakePeriodicEvent>());
            FakeTriggeredEventGenerator.SetEvents(new List<FakeTriggeredEvent>());

            //Create a new twin and iot hub client
            FakesEventsFactory = new FakeEventsFactory();
            ClientMock = (ModuleClientMock)ModuleClientProvider.GetClient();
            Twin = new FakeTwinHelper(ClientMock);
        }
    }
}
