// <copyright file="AuthenticationMethodProviderFromDevice.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.IoT.Exceptions;
using Microsoft.Azure.IoT.Agent.IoT.RestApis;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// This class returns the authentication method and the required information for the Module authentication
    /// The class retrieve the information by connection the IoT hub and querying for the device's module information
    /// </summary>
    public class AuthenticationMethodProviderFromDevice : AuthenticationMethodProviderBase
    {
        private readonly IDeviceApi _deviceAPi;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        public AuthenticationMethodProviderFromDevice(AuthenticationData authenticationData) :base(authenticationData) 
        {
            RestClient restClient = RestClient.CreateFrom(authenticationData);
            _deviceAPi = new DevicesApi(restClient);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceAPi">The device API to use</param>
        /// <param name="authenticationData">AuthenticationConfigurationData configuration</param>
        public AuthenticationMethodProviderFromDevice(IDeviceApi deviceAPi, AuthenticationData authenticationData) : base(authenticationData)
        {
            _deviceAPi = deviceAPi;
        }

        /// <summary>
        /// This method connects to the IoT Hub with the device identity.
        /// by that it queries the authentication information for its module authentication
        /// It then returns the Connection string by which the Module can perform authentication
        /// </summary>
        /// <returns>Connection String</returns>
        public override string GetConnectionString()
        {         
            JToken moduleJToken = _deviceAPi.GetDeviceAgentModule(AuthenticationData.DeviceId);
            if (moduleJToken == null)
            {
                throw new FailedToGetModuleException($"Module for device: {AuthenticationData.DeviceId} is empty");
            }
            //Note that the response represents the object Module in the SDK Microsoft.Azure.Devices
            //We do not want to include the SDK in the project so we extract the specific parameters we need
            string primaryKey = moduleJToken["authentication"]["symmetricKey"]["primaryKey"].ToString();
            return CreateModuleConnectionString(AuthenticationData.GatewayHostName, AuthenticationData.DeviceId, primaryKey);
        }
    }
}
