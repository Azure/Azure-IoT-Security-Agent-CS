// <copyright file="DevicesApi.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Net;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.IoT.RestApis
{
    /// <summary>
    /// This class implements some of the REST APIs that are supported by the DPS
    /// </summary>
    public class DpsApi : IDpsApi
    {
        /// <summary>
        /// DpsApi constructor
        /// </summary>
        /// <param name="restClient">restClient on which the apis will be sent</param>
        public DpsApi(RestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Return the device registration from the DPS
        /// The return value represents the json of the Registration object
        /// </summary>
        /// <param name="idScope">the ID Scope of the DPS service</param>
        /// <param name="registrationId">the registration ID of the device</param>
        /// <returns>JToken of the Registration object</returns>
        public JToken GetDeviceRegistration(string idScope, string registrationId)
        {
            string url = $"https://{GlobalDpsHostName}/{idScope}/registrations/{registrationId}?{ApiVersion}";
            SimpleLogger.Debug($"Sending DPS registration query to ID scope: {idScope}, with registration ID: {registrationId}");
            string requestContent = $"{{\"registrationId\":\"{registrationId}\"}}";
            try
            {
                HttpStatusCode status = _restClient.SendPostRequest(url, requestContent, out string response);
                if (status != HttpStatusCode.OK)
                    throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, $"Unexpected server response: {response}");

                JToken moduleJtoken = null;
                if (!string.IsNullOrEmpty(response))
                {
                    moduleJtoken = JToken.Parse(response);
                }

                return moduleJtoken;
            }
            catch (WebException ex)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, $"Couldn't get device registration, error: {ex.Message}");
            }
        }

        private readonly RestClient _restClient;
        private const string ApiVersion = "api-version=2019-03-31";
        private const string GlobalDpsHostName = "global.azure-devices-provisioning.net";
    }
}
