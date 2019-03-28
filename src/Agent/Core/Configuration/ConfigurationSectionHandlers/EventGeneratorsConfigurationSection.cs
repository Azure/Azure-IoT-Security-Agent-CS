// <copyright file="EventGeneratorsConfigurationSection.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers
{
    /// <summary>
    /// Handler for the EventGenrators configuration section in the App.Config
    /// </summary>
    public sealed class EventGeneratorsConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Get the App.Config EventGenerators configuration section
        /// </summary>
        public static EventGeneratorsConfigurationSection Configuration =>
            ConfigurationManager.GetSection("EventGenerators") as EventGeneratorsConfigurationSection;

        /// <summary>
        /// Get this event generator configuration elements collection
        /// </summary>
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        private EventGeneratorsConfigElementsCollection ConfigurationCollection => (EventGeneratorsConfigElementsCollection) this[""];

        /// <summary>
        /// Get all event genrator configurations from this configuration section
        /// </summary>
        public IEnumerable<EventGeneratorsConfig> ConfigurationElements =>
            ConfigurationCollection.Cast<EventGeneratorsConfig>();
    }
}