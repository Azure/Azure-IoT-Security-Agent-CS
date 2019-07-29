﻿// <copyright file="DevicesApi.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Exceptions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.IoT.RestApis
{
    /// <summary>
    /// This class implements some of the APIs that are supported by the Microsoft.Azure.Devices sdk
    /// The APIs are sent using rest protocol
    /// </summary>
    public class DevicesApi : IDeviceApi
    {
        /// <summary>
        /// DevicesApi constructor
        /// </summary>
        /// <param name="restClient">restClient on which the apis will be sent</param>
        public DevicesApi(RestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Return the agent module of the given device id
        /// The return value represents the json of the Module
        /// </summary>
        /// <param name="deviceId">the device id for which we search the module</param>
        /// <returns>JToken of the Module</returns>
        public JToken GetDeviceAgentModule(string deviceId)
        {
            return GetDeviceAgentModule(deviceId, LocalIoTHubConfiguration.Authentication.ModuleName);
        }

        /// <summary>
        /// Return the Module that is corresponded to the given moduleName of the given device id
        /// The return value represents the json of the Module
        /// </summary>
        /// <param name="deviceId">the device id for which we search the module</param>
        /// <param name="moduleName">the module name we are looking for</param>
        /// <returns>JToken of the Module or null if no module was returned</returns>
        public JToken GetDeviceAgentModule(string deviceId, string moduleName)
        {
            string url = $"https://{_restClient.GatewayHostname}/devices/{deviceId}/modules/{moduleName}?{ApiVersion}";
            SimpleLogger.Debug($"Sending GetDeviceModule to URL: {url}");
            try
            {
                HttpStatusCode status = _restClient.SendGetRequest(url, out string response);

                if (status == HttpStatusCode.Unauthorized)
                    throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Unauthorized, "Validate authentication configuration");
                if (status == HttpStatusCode.NotFound)
                    throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.NotFound, "Validate authentication configuration");
                if (status != HttpStatusCode.OK)
                    throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, response);

                JToken moduleJtoken = null;
                if (!string.IsNullOrEmpty(response))
                {
                    moduleJtoken = JToken.Parse(response);
                }

                return moduleJtoken;
            }
            catch (WebException ex)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, $"Couldn't get module from IoThub, error: {ex.Message}");
            }
        }

        private readonly RestClient _restClient;
        private const string ApiVersion = "api-version=2018-06-30";
    }
}
