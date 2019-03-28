// <copyright file="FakeSnapshotEventGenerator.cs" company="Microsoft">
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
    /// Fake snapshot event generator
    /// The generator returns the events stored in Events
    /// </summary>
    public class FakeSnapshotEventGenerator : SnapshotEventGenerator
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<FakePeriodicEvent>();

        /// <summary>
        /// The fake events this generator generates
        /// </summary>
        public static IEnumerable<FakePeriodicEvent> Events { get; private set; } = new List<FakePeriodicEvent>();

        /// <summary>
        /// Set new fake generated events
        /// </summary>
        /// <param name="events">Fake events</param>
        public static void SetEvents(IEnumerable<FakePeriodicEvent> events)
        {
            Events = events.ToList();
        }

        /// <inheritdoc />
        protected override List<IEvent> GetEventsImpl()
        {
            IEnumerable<IEvent> ret = Events.ToList();
            return ret.ToList();
        }
    }
}