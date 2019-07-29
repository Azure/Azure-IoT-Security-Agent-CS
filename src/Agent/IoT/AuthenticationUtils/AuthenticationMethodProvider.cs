// <copyright file="AuthenticationMethodProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// This class provides the authentication method and the corresponding information
    /// for the authentication of the Module
    /// Currently the only authentication method supported by the Module is by SymmetricKey (SAS token)
    /// </summary>
    public static class AuthenticationMethodProvider
    {
        /// <summary>
        /// Supported authentication types
        /// </summary>
        public enum AuthenticationType
        {
            /// <summary>
            /// SaS token using SymmetricKey
            /// </summary>
            SymmetricKey,
            /// <summary>
            /// Self signed certificate
            /// </summary>
            SelfSignedCertificate
        }

        /// <summary>
        /// The identity that provides the authentication information
        /// </summary>
        public enum AuthenticationIdentity
        {
            /// <summary>
            /// The DPS will provide the authentication information for the module
            /// </summary>
            DPS,

            /// <summary>
            /// The device will provide the authentication information for the module
            /// </summary>
            Device,

            /// <summary>
            /// The Module itself provides the authentication information
            /// </summary>
            Module
        }

        /// <summary>
        /// The location from which the certificate will be loaded
        /// </summary>
        public enum CertificateLocation
        {
            /// <summary>
            /// Local file
            /// </summary>
            LocalFile,

            /// <summary>
            /// Certificate store
            /// File content should match <see cref="CertificateFromStoreData"/>
            /// </summary>
            Store
        }

        /// <summary>
        /// Returns the connection string of the module
        /// The module can perform authentication with this value
        /// </summary>
        /// <returns>connection string</returns>
        public static string GetModuleConnectionString()
        {
            if (string.IsNullOrEmpty(_moduleConnectionString))
            {
                _moduleConnectionString = ProvideModuleConnectionString();
            }
            return _moduleConnectionString;
        }

        /// <summary>
        /// Recreates and returns the connection string of the module
        /// </summary>
        /// <returns>connection string</returns>
        public static string GetNewModuleConnectionString()
        {
            _moduleConnectionString = ProvideModuleConnectionString();
            return _moduleConnectionString;
        }

        /// <summary>
        /// Return the module connection string
        /// Retrieving the information from the module configuration or by connecting the IoT hub and asking for it
        /// </summary>
        /// <returns>Connection string</returns>
        private static string ProvideModuleConnectionString()
        {
            AuthenticationData authConfig = LocalIoTHubConfiguration.Authentication;

            AuthenticationMethodProviderBase authenticationMethodProvider;

            switch (authConfig.Identity)
            {
                case AuthenticationIdentity.Module:
                    authenticationMethodProvider = new AuthenticationMethodProviderFromModule(authConfig);
                    break;
                case AuthenticationIdentity.Device:
                    authenticationMethodProvider = new AuthenticationMethodProviderFromDevice(authConfig);
                    break;
                case AuthenticationIdentity.DPS:
                    authenticationMethodProvider = new AuthenticationMethodProviderFromDPS(authConfig);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(paramName: "identity", message: "Authentication Identity error");
            }

            return authenticationMethodProvider.GetConnectionString();
        }

        private static string _moduleConnectionString;
    }
}





