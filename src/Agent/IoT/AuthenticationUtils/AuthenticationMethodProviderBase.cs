// <copyright file="AuthenticationMethodProviderBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using System;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// Base class for classes that provide authentication method and information
    /// </summary>
    public abstract class AuthenticationMethodProviderBase
    {
        /// <summary>
        /// This function returns the connection string of the module
        /// By this connection string the module should be able to perform authentication to th IoT Hub
        /// </summary>
        /// <returns>Connection String</returns>
        public abstract string GetConnectionString();

        /// <summary>
        /// Implement the required validation of the configuration
        /// </summary>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        /// <exception cref="MisconfigurationException">throw upon misconfiguration</exception>
        public abstract void ValidateConfiguration(AuthenticationData authenticationData);


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        /// <exception cref="MisconfigurationException"></exception>
        protected AuthenticationMethodProviderBase(AuthenticationData authenticationData)
        {
            AuthenticationData = authenticationData;
        }

        /// <summary>
        /// AuthenticationData configuration
        /// </summary>
        protected AuthenticationData AuthenticationData { get; }

        /// <summary>
        /// Creates the Connection String formatted as it is expected by the Microsoft.Azure.Clients SDK
        /// </summary>
        /// <param name="gatewayHostName">Gateway FQDN</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="primaryKey">Primary key</param>
        /// <returns>Connection String formatted as it is expected by the Microsoft.Azure.Clients SDK</returns>
        protected string CreateModuleConnectionString(string gatewayHostName, string deviceId, string primaryKey)
        {
            var authMethod =
                AuthenticationMethodFactory.CreateAuthenticationWithRegistrySymmetricKey(deviceId, AuthenticationData.ModuleName, primaryKey);
            var builder = IotHubConnectionStringBuilder.Create(gatewayHostName, authMethod);

            return builder.ToString();
        }
    }
}
