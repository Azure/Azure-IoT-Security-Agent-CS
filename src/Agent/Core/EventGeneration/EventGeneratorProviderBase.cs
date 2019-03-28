// <copyright file="EventGeneratorProviderBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// A base class for the various implementations of EventGeneratorProvider in different platforms
    /// </summary>
    public abstract class EventGeneratorProviderBase : IEventGeneratorProvider
    {
        /// <summary>
        /// The list of resolved event generators
        /// </summary>
        protected readonly List<IEventGenerator> EventGenerators = new List<IEventGenerator>();

        /// <summary>
        /// A flag indicating if the resolution list is already loaded
        /// </summary>
        protected bool IsLoaded = false;

        /// <summary>
        /// Returns all resolved event generators
        /// </summary>
        /// <returns>The list of event generators</returns>
        public List<IEventGenerator> GetAll()
        {
            if (!IsLoaded)
            {
                Load();
            }

            return EventGenerators;
        }

        /// <summary>
        /// Resolves and loads all registered event generators
        /// </summary>
        protected abstract void Load();
    }
}