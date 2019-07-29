// <copyright file="LocalIoTHubConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;

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
        public static AuthenticationData Authentication { get; }

        /// <summary>
        /// IoT interface related configuration
        /// </summary>
        public static IoTInterfaceConfig IotInterface { get; }

        static LocalIoTHubConfiguration()
        {
            try
            {
                Authentication = new AuthenticationData(ConfigurationManager.GetSection("Authentication") as NameValueCollection);
            }
            catch (ArgumentNullException ex)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.MissingConfiguration, $"Key: {ex.ParamName}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.CantParseConfiguration, $"Key: {ex.ParamName}");
            }

            try
            {
                IotInterface = new IoTInterfaceConfig(ConfigurationManager.GetSection("ExternalInterface") as NameValueCollection);
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
