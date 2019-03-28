// <copyright file="RemoteSecurityModuleConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json;
using System.ComponentModel;

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
        [JsonProperty(PropertyName = "eventPriorityConnectedHardware", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ConnectedHardware { get; set; }

        /// <summary>
        /// ListeningPortsSnapshot event with its default priority
        /// </summary>
        [DefaultValue("High")]
        [JsonProperty(PropertyName = "eventPriorityListeningPorts", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ListeningPorts { get; set; }

        /// <summary>
        /// ProcessCreate event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityProcessCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ProcessCreate { get; set; }

        /// <summary>
        /// ProcessTerminate event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityProcessTerminate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ProcessTerminate { get; set; }

        /// <summary>
        /// SystemInformation event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPrioritySystemInformation", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority SystemInformation { get; set; }

        /// <summary>
        /// LocalUsers event with its default priority
        /// </summary>
        [DefaultValue("High")]
        [JsonProperty(PropertyName = "eventPriorityLocalUsers", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority LocalUsers { get; set; }

        /// <summary>
        /// Login event with its default priority
        /// </summary>
        [DefaultValue("High")]
        [JsonProperty(PropertyName = "eventPriorityLogin", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority Login { get; set; }

        /// <summary>
        /// ConnectionCreate event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityConnectionCreate", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ConnectionCreate { get; set; }

        /// <summary>
        /// FirewallConfiguration event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityFirewallConfiguration", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority FirewallConfiguration { get; set; }

        /// <summary>
        /// OSBaseline event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityOSBaseline", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority OSBaseline { get; set; }
    }
}
