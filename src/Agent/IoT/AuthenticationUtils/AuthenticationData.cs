// <copyright file="AuthenticationData.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Logging;
using System.Collections.Specialized;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Utils;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// AuthenticationData holds the required information for authentication 
    /// </summary>
    public class AuthenticationData
    {
        /// <summary>
        /// The identity from which we would extract the authentication information if the module
        /// Identity could be the Device or the Module itself
        /// </summary>
        public AuthenticationMethodProvider.AuthenticationIdentity Identity { get; set; }

        /// <summary>
        /// The authentication type e.g. certificate/sas token
        /// </summary>
        public AuthenticationMethodProvider.AuthenticationType Type { get; }

        /// <summary>
        /// Specifies where the certificate should be loaded from (Local/Store)
        /// Relevant only when <see cref="Type"/> is set to <see cref="AuthenticationMethodProvider.AuthenticationType.SelfSignedCertificate"/>
        /// </summary>
        public AuthenticationMethodProvider.CertificateLocation? CertificateLocation { get; }

        /// <summary>
        /// The file path to the required data of the AuthenticationType
        /// The file might contains for example the certificate (for certificate Type) or the connection string (for Sas token Type)
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The device Id of the Module
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets the name of the module the agent is interacting with
        /// </summary>
        public string ModuleName { get; }

        /// <summary>
        /// The fixed name of the CertificateLocation key
        /// </summary>
        public const string CertificateLocationKindKey = "certificateLocationKind";

        /// <summary>
        /// The FQDN of the gateway e.g. iotTestHub.azure.net
        /// </summary>
        public string GatewayHostName { get; set; }

        /// <summary>
        /// The ID Scope of the DPS service
        /// </summary>
        public string IdScope { get; }

        /// <summary>
        /// The registration ID of the device, as specified in the DPS
        /// </summary>
        public string RegistrationId { get; }

        /// <summary>
        /// The fixed name of the FilePath key
        /// </summary>
        public const string FilePathKey = "filePath";
        /// <summary>
        /// The fixed name of the DeviceId key
        /// </summary>
        public const string DeviceIdKey = "deviceId";

        /// <summary>
        /// The fixed name of the Gateway key
        /// </summary>
        public const string GatewayHostNameKey = "gatewayHostName";

        /// <summary>
        /// The fixed name of the Identity key
        /// </summary>
        public const string IdentityKey = "identity";

        /// <summary>
        /// The fixed name of the Type key
        /// </summary>
        public const string TypeKey = "type";

        /// <summary>
        /// The fixed name of the ModuleName key
        /// </summary>
        public const string ModuleNameKey = "moduleName";

        /// <summary>
        /// The fixed name of the IdScope key
        /// </summary>
        public const string IdScopeKey = "idScope";

        /// <summary>
        /// The fixed name of the RegistrationId key
        /// </summary>
        public const string RegistrationIdKey = "registrationId";

        /// <summary>
        /// Constructor that is initialized from a given configuration 
        /// </summary>
        /// <param name="nameValueCollection">nameValueCollection represents the user configuration</param>
        public AuthenticationData(NameValueCollection nameValueCollection)
        {
            FilePath = nameValueCollection.GetStringValueThrowOnFail(FilePathKey);
            ModuleName = nameValueCollection.GetStringValueThrowOnFail(ModuleNameKey);
            Type = nameValueCollection.GetEnumValueThrowOnFail<AuthenticationMethodProvider.AuthenticationType>(TypeKey);
            Identity = nameValueCollection.GetEnumValueThrowOnFail<AuthenticationMethodProvider.AuthenticationIdentity>(IdentityKey);
            if (Identity == AuthenticationMethodProvider.AuthenticationIdentity.DPS)
            {
                IdScope = nameValueCollection.GetStringValueThrowOnFail(IdScopeKey);
                RegistrationId = nameValueCollection.GetStringValueThrowOnFail(RegistrationIdKey);
                GatewayHostName = GlobalDpsHostName;
            } else
            {
                DeviceId = nameValueCollection.GetStringValueThrowOnFail(DeviceIdKey);
                GatewayHostName = nameValueCollection.GetStringValueThrowOnFail(GatewayHostNameKey);
            }

            bool isSelfSignedCertType = Type == AuthenticationMethodProvider.AuthenticationType.SelfSignedCertificate;
            if (isSelfSignedCertType)
                CertificateLocation = nameValueCollection.GetEnumValueThrowOnFail<AuthenticationMethodProvider.CertificateLocation>(CertificateLocationKindKey);
            if (isSelfSignedCertType && Identity == AuthenticationMethodProvider.AuthenticationIdentity.Module)
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, "Only SymmetricKey method is valid for module identity");

            SimpleLogger.Debug($"Configuration is: type {Type} {(isSelfSignedCertType ? $"CertificationLocation {CertificateLocation}" : "")}" +
                $" Identity {Identity} FilePath: {FilePath} DeviceId: {DeviceId} GatewayHostName: {GatewayHostName}");
        }

        /// <summary>
        /// Copy C-tor
        /// </summary>
        /// <param name="authenticationData">The AuthenticationData object to copy</param>
        public AuthenticationData(AuthenticationData authenticationData)
        {
            FilePath = authenticationData.FilePath;
            ModuleName = authenticationData.ModuleName;
            Identity = authenticationData.Identity;
            IdScope = authenticationData.IdScope;
            RegistrationId = authenticationData.RegistrationId;
            DeviceId = authenticationData.DeviceId;
            GatewayHostName = authenticationData.GatewayHostName;
            Type = authenticationData.Type;
            CertificateLocation = authenticationData.CertificateLocation;
        }

        private const string GlobalDpsHostName = "global.azure-devices-provisioning.net";
    }
}

