// <copyright file="IoTHubInterfaceBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients;
using Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients;

namespace Microsoft.Azure.IoT.Agent.IoT.Providers
{
    /// <summary>
    /// Facade to the IoTHub external interface
    /// </summary>
    public abstract class IoTHubInterfaceBase : IExternalInterface
    {
        /// <inheritdoc />
        public abstract IRemoteConfigurationProvider RemoteConfigurationProvider { get; }

        /// <inheritdoc />
        public IMessagingClient ExternalClient => IotHubMessagingClient.Instance;

        /// <inheritdoc />
        public void Dispose()
        {
            IotHubMessagingClient.DisposeInstance();
        }
    }

}
