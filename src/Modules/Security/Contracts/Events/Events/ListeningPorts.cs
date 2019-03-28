// <copyright file="ListeningPorts.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <summary>
    /// Snapshot event that sends all tcp open ports on the machine in the priodic check time
    /// </summary>
    public class ListeningPorts : SecurityPeriodicEvent<ListeningPortsPayload>
    {
        /// <inheritdoc />
        public ListeningPorts(EventPriority priority, params ListeningPortsPayload[] payloads)
            : base(priority, payloads)
        {
        }
    }
}
