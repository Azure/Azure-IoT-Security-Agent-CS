// <copyright file="LocalConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;

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
        public static string AgentVersion { get; } 

        /// <summary>
        /// Gets the event generators configurations
        /// </summary>
        public static List<EventGeneratorsConfig> EventGenerators { get; }

        /// <summary>
        /// Agent related configuration
        /// </summary>
        public static GeneralConfig General { get; } 

        /// <summary>
        /// External interface related configuration
        /// </summary>
        public static ExternalInterfaceConfig ExternalInterface { get; }

        /// <summary>
        /// Initiate fields 
        /// </summary>
        static LocalConfiguration()
        {
            try
            {
                General = new GeneralConfig(ConfigurationManager.GetSection("General") as NameValueCollection);
                ExternalInterface = new ExternalInterfaceConfig(ConfigurationManager.GetSection("ExternalInterface") as NameValueCollection);
                EventGenerators = EventGeneratorsConfigurationSection.Configuration.ConfigurationElements.ToList();
                AgentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (ArgumentNullException ex)
            {
                throw new AgentException(ExceptionCodes.LocalConfiguration, ExceptionSubCodes.MissingConfiguration, $"Key: {ex.ParamName}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new AgentException(ExceptionCodes.LocalConfiguration, ExceptionSubCodes.CantParseConfiguration, $"Key: {ex.ParamName}");
            }
        }
    }
}