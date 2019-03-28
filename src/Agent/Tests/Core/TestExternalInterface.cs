// <copyright file="IoTHubInterfaceFacade.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Providers;

namespace Microsoft.Azure.IoT.Agent.Core.Tests
{
    /// <inheritdoc />
    public class TestExternalInterface : IoTHubInterfaceBase
    {
        /// <inheritdoc />
        public override IRemoteConfigurationProvider RemoteConfigurationProvider { get; } = new IoTHubTwinConfigurationProvider<TestIotConfiguration>();
    }
}
