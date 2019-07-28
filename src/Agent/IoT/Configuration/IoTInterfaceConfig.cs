// <copyright file="IoTInterfaceConfig.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers;
using System;
using System.Collections.Specialized;
using Microsoft.Azure.IoT.Agent.Core.Utils;

namespace Microsoft.Azure.IoT.Agent.IoT.Configuration
{
    /// <summary>
    /// Configuration for the agent's IoT interface
    /// </summary>
    public class IoTInterfaceConfig : ExternalInterfaceConfig
    {
        private const string TransportTypeKey = "transportType";
        private const string RemoteConfigurationObjectKey = "RemoteConfigurationObject";

        /// <summary>
        /// Gets the transport type to use
        /// </summary>
        public TransportType TransportType { get; private set; }

        /// <summary>
        /// Gets the remote (twin) configuration object name
        /// </summary>
        public string RemoteConfigurationObject { get; private set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public IoTInterfaceConfig(NameValueCollection nameValueCollection) : base(nameValueCollection)
        {
            TransportType = nameValueCollection.GetEnumValueThrowOnFail<TransportType>(TransportTypeKey);
            RemoteConfigurationObject = nameValueCollection.GetStringValueThrowOnFail(RemoteConfigurationObjectKey);
        }
    }
}
