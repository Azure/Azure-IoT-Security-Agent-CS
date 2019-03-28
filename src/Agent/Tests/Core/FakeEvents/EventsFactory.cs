// <copyright file="EventsFactory.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <summary>
    /// Factory for fake events
    /// </summary>
    public class FakeEventsFactory
    {
        /// <summary>
        /// Creates a fake operational event with the given payload
        /// If the given payload is null creates a fake operational event with the default payload
        /// </summary>
        /// <param name="payload">Generated events payload</param>
        /// <returns>Fake operational event</returns>
        public FakeOperationalEvent CreateFakeOperationalEvent(FakeEventPayload payload = null)
        {
            var eventPayload = payload ?? FakeEventPayload.DefaultFakeEventPayload;

            return new FakeOperationalEvent(eventPayload);
        }

        /// <summary>
        /// Creates a fake priodic event with the given payload
        /// If the given payload is null creates a fake periodic event with the default payload
        /// </summary>
        /// <param name="priority">Generated events priority</param>
        /// <param name="payload">Generated events payload</param>
        /// <returns>Fake periodic event</returns>
        public FakePeriodicEvent CreateFakePeriodicEvent(EventPriority priority, FakeEventPayload payload = null)
        {
            var eventPayload = payload ?? FakeEventPayload.DefaultFakeEventPayload;

            return new FakePeriodicEvent(priority, eventPayload);
        }

        /// <summary>
        /// Creates a fake triggerd event with the given payload
        /// If the given payload is null creates a fake triggerd event with the default payload
        /// <param name="priority">Generated events priority</param>
        /// <param name="payload">Generated events payload</param>
        /// <returns>Fake operational event</returns>
        /// </summary>
        public FakeTriggeredEvent CreateFakeTriggeredEvent(EventPriority priority, FakeEventPayload payload = null)
        {
            var eventPayload = payload ?? FakeEventPayload.DefaultFakeEventPayload;

            return new FakeTriggeredEvent(priority, eventPayload);
        }
    }
}