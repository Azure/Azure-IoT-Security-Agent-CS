// <copyright file="SecurityIoTHubInterface.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Providers;

namespace Microsoft.Azure.Security.IoT.Agent.Common
{
    /// <summary>
    /// Facade to the IoTHub external interface, with security module configuration
    /// </summary>
    public class SecurityIoTHubInterface : IoTHubInterfaceBase
    {
        /// <inheritdoc />
        public override IRemoteConfigurationProvider RemoteConfigurationProvider { get; } = new IoTHubTwinConfigurationProvider<RemoteSecurityModuleConfiguration>();
    }
}
