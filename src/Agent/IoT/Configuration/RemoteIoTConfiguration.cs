// <copyright file="RemoteIoTConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Microsoft.Azure.IoT.Agent.IoT.Configuration
{
    /// <summary>
    /// Extension of <see cref="RemoteConfiguration"/> that holds IoT related configurations
    /// </summary>
    public class RemoteIoTConfiguration : RemoteConfiguration
    {
        /// <summary>
        /// The Azure resource id of the IoT hub, needs to be added to the message for backend purposes
        /// There is no default value
        /// It should always be included in the remote configuration (??)
        /// </summary>
        [DefaultValue("")]
        [JsonProperty(PropertyName = "hubResourceId", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public string HubResourceId { get; set; }
    }
}
