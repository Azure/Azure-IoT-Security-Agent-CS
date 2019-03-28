// <copyright file="OSBaseline.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class OSBaseline : SecurityPeriodicEvent<BaselinePayload>
    {
        /// <inheritdoc />
        public OSBaseline(EventPriority priority, params BaselinePayload[] payloads)
            : base(priority, payloads)
        {
        }
    }
}