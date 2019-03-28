// <copyright file="LocalConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration
{
    /// <summary>
    /// Provides configuration from local configuration file
    /// </summary>
    public static class LocalConfiguration
    {
        /// <summary>
        /// Gets the agent version
        /// </summary>
        public static string AgentVersion => GetAgentVersion();

        /// <summary>
        /// Gets the event generators configurations
        /// </summary>
        public static List<EventGeneratorsConfig> EventGenerators =>
            EventGeneratorsConfigurationSection.Configuration.ConfigurationElements.ToList();

        /// <summary>
        /// Agent related configuration
        /// </summary>
        public static GeneralConfig General => new GeneralConfig(ConfigurationManager.GetSection("General") as NameValueCollection);

        public static ExternalInterfaceConfig ExternalInterface => new ExternalInterfaceConfig(ConfigurationManager.GetSection("ExternalInterface") as NameValueCollection);

        /// <summary>
        /// Gets the assembly version of the agent
        /// </summary>
        /// <returns>agent version</returns>
        private static string GetAgentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}