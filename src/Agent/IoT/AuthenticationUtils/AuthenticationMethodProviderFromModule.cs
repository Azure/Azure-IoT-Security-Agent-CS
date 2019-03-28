// <copyright file="AuthenticationMethodProviderFromModule.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.Security.IoT.Agent.Common.Utils;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// This class returns the authentication method and the required information for the Module authentication
    /// The class retrieve the information from a configuration
    /// </summary>
    public class AuthenticationMethodProviderFromModule : AuthenticationMethodProviderBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        public AuthenticationMethodProviderFromModule(AuthenticationData authenticationData) : base(authenticationData)
        {
        }

        /// <summary>
        /// Returns the connection string from the configuration file
        /// </summary>
        /// <returns>Connection String</returns>
        public override string GetConnectionString()
        {
            string key = ConfigurationUtils.GetSymmetricKeyFromFile(AuthenticationData.FilePath);
            return CreateModuleConnectionString(AuthenticationData.GatewayHostName, AuthenticationData.DeviceId, key);
        }

        /// <summary>
        /// Validate the AuthenticationConfigurationData as it arrived from the configuration
        /// </summary>
        /// <param name="authenticationData">AuthenticationConfigurationData</param>
        /// <exception cref="MisconfigurationException"></exception>
        public override void ValidateConfiguration(AuthenticationData authenticationData)
        {
            if (authenticationData.Type != AuthenticationMethodProvider.AuthenticationType.SymmetricKey)
            {
                throw new MisconfigurationException($"Authentication type {authenticationData.Type} is not supported for the module");
            }

            if (string.IsNullOrEmpty(authenticationData.FilePath))
            {
                throw new MisconfigurationException("FilePath cannot be empty");
            }
        }
    }
}
