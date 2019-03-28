// <copyright file="FakeTriggeredEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <summary>
    /// Fake triggerd event generator
    /// The generator returns the events stored in Events
    /// </summary>
    public class FakeTriggeredEventGenerator : EventGenerator
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<FakeTriggeredEvent>();

        /// <summary>
        /// The fake events this generator generates
        /// </summary>
        public static IEnumerable<FakeTriggeredEvent> Events;

        /// <summary>
        /// Set new fake generated events
        /// </summary>
        /// <param name="events">Fake events</param>
        public static void SetEvents(IEnumerable<FakeTriggeredEvent> events)
        {
            Events = events.ToList();
        }

        /// <inheritdoc />
        public override IEnumerable<IEvent> GetEvents()
        {
            return Events.ToList();
        }
    }
}