// <copyright file="FakeOperationalEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <summary>
    /// Fake operational event generator
    /// The generator returns the events stored in Events
    /// </summary>
    public class FakeOperationalEventGenerator : IEventGenerator
    {
        /// <inheritdoc />
        public EventPriority Priority => AgentConfiguration.GetEventPriority<FakeOperationalEvent>();

        /// <summary>
        /// The fake events this generator generates
        /// </summary>
        public static IEnumerable<FakeOperationalEvent> Events { get; private set; } = new List<FakeOperationalEvent>();

        /// <summary>
        /// Set new fake generated events
        /// </summary>
        /// <param name="events">Fake events</param>
        public static void SetEvents(IEnumerable<FakeOperationalEvent> events)
        {
            Events = events.ToList();
        }

        /// <inheritdoc />
        public IEnumerable<IEvent> GetEvents()
        {
            return Events.ToList();
        }
    }
}


