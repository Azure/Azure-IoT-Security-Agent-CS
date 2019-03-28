// <copyright file="ConfigurationErrorPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads
{
    /// <summary>
    /// Configuration error payload
    /// </summary>
    public class ConfigurationErrorPayload : Payload
    {
        /// <summary>
        /// The name of the configuration
        /// </summary>
        public string ConfigurationName { get; set; }

        /// <summary>
        /// Error type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfigurationErrorType ErrorType { get; set; }

        /// <summary>
        /// What configuration is in use
        /// </summary>
        public string UsedConfiguration { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
    }
}
