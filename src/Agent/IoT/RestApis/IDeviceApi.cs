// <copyright file="IDeviceApi.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.IoT.RestApis
{
    /// <summary>
    /// Interface for device API
    /// abstract functions of the supported APIs
    /// </summary>
    public interface IDeviceApi
    {
        /// <summary>
        /// Return the agent module of the given device id
        /// The return value represents the json of the Module
        /// </summary>
        /// <param name="deviceId">the device id for which we search the module</param>
        /// <returns>JToken of the Module</returns>
        JToken GetDeviceAgentModule(string deviceId);
    }
}
