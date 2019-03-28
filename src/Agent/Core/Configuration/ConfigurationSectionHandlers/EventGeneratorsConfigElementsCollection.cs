// <copyright file="EventGeneratorsConfigElementsCollection.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Configuration;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers
{
    /// <summary>
    /// A configuration collection of event generator configuration elements
    /// </summary>
    public sealed class EventGeneratorsConfigElementsCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new EventGeneratorsConfig elemnt
        /// </summary>
        /// <returns>New event configuration element</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new EventGeneratorsConfig();
        }

        /// <summary>
        /// Returns the key of the given element
        /// </summary>
        /// <param name="element">Event generator configuration elemnt</param>
        /// <returns>The name of the event generator</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EventGeneratorsConfig) element).Name;
        }
    }
}