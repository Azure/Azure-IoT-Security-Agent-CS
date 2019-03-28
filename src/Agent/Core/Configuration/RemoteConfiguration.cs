// <copyright file="RemoteConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration
{
    /// <summary>
    /// Represents the agent RemoteConfiguration - hard coded keys and their default values
    /// </summary>
    public class RemoteConfiguration
    {
        /// <summary>
        /// The interval upon which messages with level "High" should be sent 
        /// </summary>
        [JsonProperty(PropertyName = "highPriorityMessageFrequency", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue("00:07:00")]
        public TimeSpan HighPriorityMessageFrequency { get; set; }

        /// <summary>
        /// The interval upon which messages with level "Low" should be sent 
        /// </summary>
        [DefaultValue("05:00:00")]
        [JsonProperty(PropertyName = "lowPriorityMessageFrequency", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan LowPriorityMessageFrequency { get; set; }

        /// <summary>
        /// The interval upon which messages of type "snapshot" should be sent 
        /// </summary>
        [DefaultValue("13:00:00")]
        [JsonProperty(PropertyName = "snapshotFrequency", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan SnapshotFrequency { get; set; }

        /// <summary>
        /// The maximum cache size for the event queues
        /// </summary>
        [DefaultValue("2560000")]
        [JsonProperty(PropertyName = "maxLocalCacheSizeInBytes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Ignore)]
        public uint MaxLocalCacheSize { get; set; }

        /// <summary>
        /// Maximum size of the message that is sent to the hub
        /// </summary>
        [DefaultValue("204800")]
        [JsonProperty(PropertyName = "maxMessageSizeInBytes", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public uint MaxMessageSize { get; set; }

        /// <summary>
        /// Diagnostic event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityDiagnostic", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority Diagnostic { get; set; }

        /// <summary>
        /// ConfigurationError event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityConfigurationError", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority ConfigurationError { get; set; }

        /// <summary>
        /// DroppedEventsStatistics event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityDroppedEventsStatistics", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority DroppedEventsStatistics { get; set; }

        /// <summary>
        /// MessageStatistics event with its default priority
        /// </summary>
        [DefaultValue("Low")]
        [JsonProperty(PropertyName = "eventPriorityMessageStatistics", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public EventPriority MessageStatistics { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, new StringEnumConverter());
        }      
    }
}
