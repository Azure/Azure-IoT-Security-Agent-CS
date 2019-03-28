// <copyright file="EventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// Event generator base class that provides common logic
    /// </summary>
    public abstract class EventGenerator : IEventGenerator
    {
        /// <inheritdoc />
        public abstract EventPriority Priority { get; }

        /// <inheritdoc />
        public abstract IEnumerable<IEvent> GetEvents();
    }
}