// <copyright file="SnapshotEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// A base class for snapshot event generators
    /// </summary>
    public abstract class SnapshotEventGenerator : EventGenerator
    {
        private DateTime _lastSendTime = DateTime.MinValue;

        /// <summary>
        /// Generates the snapshot
        /// </summary>
        /// <returns>A list containing the snapshot, there should be a single event in the list
        /// an empty list is returned if time since last snapshot is less than snapshot frequency configuration</returns>
        public override IEnumerable<IEvent> GetEvents()
        {
            var returnedEvents = new List<IEvent>();

            if (_lastSendTime + AgentConfiguration.RemoteConfiguration.SnapshotFrequency < DateTime.UtcNow)
            {
                returnedEvents = GetEventsImpl();
                _lastSendTime = DateTime.UtcNow;
            }

            return returnedEvents;
        }

        /// <summary>
        /// Actual implementation of get events
        /// </summary>
        /// <returns>A list containing the snapshot, there should be a single event in the list</returns>
        protected abstract List<IEvent> GetEventsImpl();
    }
}