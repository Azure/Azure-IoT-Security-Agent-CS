// <copyright file="IDeviceApi.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.IoT.RestApis
{
    /// <summary>
    /// Interface for DPS API
    /// abstract functions of the supported APIs
    /// </summary>
    public interface IDpsApi
    {
        /// <summary>
        /// Return the device registration from the DPS
        /// The return value represents the json of the Registration object
        /// </summary>
        /// <param name="idScope">the ID Scope of the DPS service</param>
        /// <param name="registrationId">the registration ID of the device</param>
        /// <returns>JToken of the Registration object</returns>
        JToken GetDeviceRegistration(string idScope, string registrationId);
    }
}
