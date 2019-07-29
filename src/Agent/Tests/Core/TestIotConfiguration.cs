// <copyright file="IoTHubInterfaceFacade.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.Azure.IoT.Agent.Core.Configuration;

namespace Microsoft.Azure.IoT.Agent.Core.Tests
{
    /// <inheritdoc />
    public class TestIotConfiguration : RemoteIoTConfiguration
    {
        /// <summary>
        /// Fake heartbeat event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityFakeOperationalEvent", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority FakeOperationalEvent { get; set; }

        /// <summary>
        /// Fake periodic event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityFakePeriodicEvent", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority FakePeriodicEvent { get; set; }

        /// <summary>
        /// Fake triggered event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityFakeTriggeredEvent", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority FakeTriggeredEvent { get; set; }
    }
}
