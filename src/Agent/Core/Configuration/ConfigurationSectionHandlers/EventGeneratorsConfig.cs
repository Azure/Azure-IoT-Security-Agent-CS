// <copyright file="EventGeneratorsConfig.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Configuration;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers
{
    /// <summary>
    /// Reperesent an EventGenerator configuration entity.
    /// This class corresponds to a <add Name="value" Dll="value"/> line in the App.Config file under EventGenerators section
    /// </summary>
    public sealed class EventGeneratorsConfig : ConfigurationElement
    {
        /// <summary>
        /// The fully qualified name of the event generator
        /// </summary>
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get => (string) base["Name"];
            set => base["Name"] = value;
        }

        /// <summary>
        /// The dll containing the event generator
        /// </summary>
        [ConfigurationProperty("Dll", IsRequired = true)]
        public string Dll
        {
            get => (string) base["Dll"];
            set => base["Dll"] = value;
        }
    }
}