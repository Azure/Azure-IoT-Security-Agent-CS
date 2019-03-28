// <copyright file="LocalUsers.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class LocalUsers : SecurityPeriodicEvent<LocalUsersPayload>
    {
        /// <inheritdoc />
        public LocalUsers(EventPriority priority, params LocalUsersPayload[] payloads) :
            base(priority, payloads)
        {
        }
    }
}