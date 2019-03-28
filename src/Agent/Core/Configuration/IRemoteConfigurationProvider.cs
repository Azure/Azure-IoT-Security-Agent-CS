// <copyright file="IRemoteConfigurationProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration
{
    /// <summary>
    /// Agent configuration failed to parse remote configuration
    /// </summary>
    /// <param name="configurations">failed to parse configurations</param>
    public delegate void FailedToParseConfiguration(IList<string> configurations);

    /// <summary>
    /// Configuration had changed
    /// </summary>
    /// <param name="newConfiguration">The new configuration</param>
    public delegate void RemoteConfigurationChangedEventHandler(RemoteConfiguration newConfiguration);

    /// <summary>
    /// Represents a <see cref="RemoteConfiguration"/> provider
    /// </summary>
    public interface IRemoteConfigurationProvider
    {
        /// <summary>
        /// Event that notifies about configuration updates
        /// </summary>
        event RemoteConfigurationChangedEventHandler RemoteConfigurationChanged;

        /// <summary>
        /// Returns the latest <see cref="RemoteConfiguration"/> state
        /// </summary>
        /// <returns></returns>
        RemoteConfiguration GetRemoteConfigurationData();

        /// <summary>
        /// Event handlers for FailedToParseConfiguration
        /// </summary>
        event FailedToParseConfiguration FailedToParseConfiguration;
    }
}
