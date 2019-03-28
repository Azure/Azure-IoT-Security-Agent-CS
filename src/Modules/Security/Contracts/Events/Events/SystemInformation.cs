// <copyright file="SystemInformation.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class SystemInformation : SecurityPeriodicEvent<SystemInformationPayload>
    {
        /// <inheritdoc />
        public SystemInformation(EventPriority priority, params SystemInformationPayload[] payloads)
            : base(priority, payloads)
        {
        }
    }
}