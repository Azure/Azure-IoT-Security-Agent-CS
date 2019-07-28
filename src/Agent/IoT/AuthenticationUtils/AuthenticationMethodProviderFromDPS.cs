// <copyright file="AuthenticationMethodProviderFromDevice.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.IoT.Exceptions;
using Microsoft.Azure.IoT.Agent.IoT.RestApis;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// This class returns the authentication method and the required information for the Module authentication
    /// The class retrieves the information by connecting to theDPS service and querying the last registration, then to the IoT hub and querying for the device's module information
    /// </summary>
    public class AuthenticationMethodProviderFromDPS : AuthenticationMethodProviderBase
    {
        private readonly IDpsApi _dpsApi;
        private readonly string _idScope;
        private readonly string _registrationId;
        private AuthenticationData _deviceAuthenticationData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        public AuthenticationMethodProviderFromDPS(AuthenticationData authenticationData) : base(authenticationData)
        {
            RestClient restClient = RestClient.CreateFrom(authenticationData);
            _deviceAuthenticationData = new AuthenticationData(authenticationData);
            _deviceAuthenticationData.Identity = AuthenticationMethodProvider.AuthenticationIdentity.Device;
            _dpsApi = new DpsApi(restClient);
            _idScope = authenticationData.IdScope;
            _registrationId = authenticationData.RegistrationId;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dpsAPi">The DPS API to use</param>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        public AuthenticationMethodProviderFromDPS(IDpsApi dpsAPi, AuthenticationData authenticationData) : base(authenticationData)
        {
            _dpsApi = dpsAPi;
        }

        /// <summary>
        /// This method connects to the IoT Hub with the device identity.
        /// by that it queries the authentication information for its module authentication
        /// It then returns the Connection string by which the Module can perform authentication
        /// </summary>
        /// <returns>Connection String</returns>
        public override string GetConnectionString()
        {
            JToken registrationToken = _dpsApi.GetDeviceRegistration(_idScope, _registrationId);
            if (registrationToken == null)
            {
                throw new FailedToGetRegistrationException($"Registration ID: \"{_registrationId}\" for ID Scope: \"{_idScope}\" could not be retrieved");
            }

            string assignedHub = registrationToken["assignedHub"].ToString();
            string deviceId = registrationToken["deviceId"].ToString();

            string status = registrationToken["status"].ToString();
            if (status != "assigned")
            {
                // TODO: handle error
            }

            _deviceAuthenticationData.GatewayHostName = assignedHub;
            _deviceAuthenticationData.DeviceId = deviceId;

            IDeviceApi deviceApi = new DevicesApi(RestClient.CreateFrom(_deviceAuthenticationData));
            JToken moduleJToken = deviceApi.GetDeviceAgentModule(deviceId);
            if (moduleJToken == null)
            {
                throw new FailedToGetModuleException($"Module for device: {deviceId} is empty");
            }
            //Note that the response represents the object Module in the SDK Microsoft.Azure.Devices
            //We do not want to include the SDK in the project so we extract the specific parameters we need
            string primaryKey = moduleJToken["authentication"]["symmetricKey"]["primaryKey"].ToString();
            return CreateModuleConnectionString(assignedHub, deviceId, primaryKey);
        }
    }
}
