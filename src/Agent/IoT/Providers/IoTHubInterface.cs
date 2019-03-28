// <copyright file="IoTHubInterfaceFacade.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;

namespace Microsoft.Azure.IoT.Agent.IoT.Providers
{
    /// <inheritdoc />
    public class IoTHubInterface : IoTHubInterfaceBase
    {
        /// <inheritdoc />
        public override IRemoteConfigurationProvider RemoteConfigurationProvider { get; } = new IoTHubTwinConfigurationProvider<RemoteIoTConfiguration>();
    }
}
