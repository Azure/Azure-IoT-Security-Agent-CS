// <copyright file="AuthenticationMethodProviderFromModule.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

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
            string key = AuthenticationFileUtils.GetBase64EncodedSymmetricKeyFromFile(AuthenticationData.FilePath);
            return CreateModuleConnectionString(AuthenticationData.GatewayHostName, AuthenticationData.DeviceId, key);
        }
    }
}
