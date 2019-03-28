// <copyright file="IExternalInterfaceFacade.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients;
using System;

namespace Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients
{
    /// <summary>
    /// Represents a facade to the external interface. Exposes functionality dependent on the external interface
    /// </summary>
    public interface IExternalInterface : IDisposable
    {
        /// <summary>
        /// Gets a provider for <see cref="RemoteConfiguration"/>
        /// </summary>
        IRemoteConfigurationProvider RemoteConfigurationProvider { get; }

        /// <summary>
        /// Gets a client to the external interface
        /// </summary>
        IMessagingClient ExternalClient { get; }
    }

}
