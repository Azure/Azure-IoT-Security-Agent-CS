// <copyright file="RemoteSecurityModuleConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json;
using System.ComponentModel;
using Microsoft.Azure.IoT.Agent.Core.Configuration;

namespace Microsoft.Azure.Security.IoT.Agent.Common
{
    /// <summary>
    /// Extension of <see cref="RemoteIoTConfiguration"/> that holds security related configurations
    /// </summary>
    public class RemoteSecurityModuleConfiguration : RemoteIoTConfiguration
    {
        /// <summary>
        /// ConnectedHardware event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityConnectedHardware", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ConnectedHardware { get; set; }

        /// <summary>
        /// ListeningPortsSnapshot event with its default priority
        /// </summary>
        [DefaultValue("High")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityListeningPorts", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ListeningPorts { get; set; }

        /// <summary>
        /// ProcessCreate event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityProcessCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ProcessCreate { get; set; }

        /// <summary>
        /// ProcessTerminate event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityProcessTerminate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ProcessTerminate { get; set; }

        /// <summary>
        /// SystemInformation event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPrioritySystemInformation", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority SystemInformation { get; set; }

        /// <summary>
        /// LocalUsers event with its default priority
        /// </summary>
        [DefaultValue("High")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityLocalUsers", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority LocalUsers { get; set; }

        /// <summary>
        /// Login event with its default priority
        /// </summary>
        [DefaultValue("High")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityLogin", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority Login { get; set; }

        /// <summary>
        /// ConnectionCreate event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityConnectionCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ConnectionCreate { get; set; }

        /// <summary>
        /// FirewallConfiguration event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityFirewallConfiguration", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority FirewallConfiguration { get; set; }

        /// <summary>
        /// OSBaseline event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "eventPriorityOSBaseline", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority OSBaseline { get; set; }

        /// <summary>
        /// Enable event aggregation for process create
        /// </summary>
        [DefaultValue(true)]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "aggregationEnabledProcessCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool ProcessCreateAgrregationEnabled { get; set; }

        /// <summary>
        /// Event aggregation interval for process create
        /// </summary>
        [DefaultValue("01:00:00")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "aggregationIntervalProcessCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan ProcessCreateAgrregationInterval { get; set; }

        /// <summary>
        /// Enable event aggregation for connection create
        /// </summary>
        [DefaultValue(true)]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "aggregationEnabledConnectionCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool ConnectionCreateAgrregationEnabled { get; set; }

        /// <summary>
        /// Event aggregation interval for connection create
        /// </summary>
        [DefaultValue("01:00:00")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "aggregationIntervalConnectionCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan ConnectionCreateAgrregationInterval { get; set; }

        /// <summary>
        /// Enable event aggregation for process terminate
        /// </summary>
        [DefaultValue(true)]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "aggregationEnabledProcessTerminate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool ProcessTerminateAggregationEnabled { get; set; }

        /// <summary>
        /// Event aggregation interval for process terminate
        /// </summary>
        [DefaultValue("01:00:00")]
        [JsonConverter(typeof(RemoteConfigurationPnPConverter))]
        [JsonProperty(PropertyName = "aggregationIntervalProcessTerminate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan ProcessTerminateAggregationInterval { get; set; }
    }
}
