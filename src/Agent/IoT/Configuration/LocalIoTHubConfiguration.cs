// <copyright file="LocalIoTHubConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using System.Collections.Specialized;
using System.Configuration;

namespace Microsoft.Azure.IoT.Agent.IoT.Configuration
{
    /// <summary>
    /// Represents local configuration values related to the IoTHub
    /// </summary>
    public class LocalIoTHubConfiguration
    {
        /// <summary>
        /// Authentication related configuration
        /// </summary>
        public static AuthenticationData Authentication => new AuthenticationData(ConfigurationManager.GetSection("Authentication") as NameValueCollection);

        public static IoTInterfaceConfig IotInterface => new IoTInterfaceConfig(ConfigurationManager.GetSection("ExternalInterface") as NameValueCollection);
    }
}
