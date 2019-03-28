// <copyright file="ConnectedHardware.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class ConnectedHardware : SecurityPeriodicEvent<ConnectedHardwarePayload>
    {
        /// <inheritdoc />
        public ConnectedHardware(EventPriority priority, params ConnectedHardwarePayload[] payloads)
            : base(priority, payloads)
        {
        }
    }
}