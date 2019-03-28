// <copyright file="Login.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <summary>
    /// Represents a login event
    /// </summary>
    public class Login : SecurityTriggeredEvent<LoginPayload>
    {
        /// <inheritdoc />
        public override DateTime EventGenerationTime { get; }

        /// <summary>
        /// Get an instance of a login event
        /// </summary>
        /// <param name="priority">The priority of the event</param>
        /// <param name="payload">The payload of the event</param>
        /// <param name="eventTime">The time the original login attempt was made</param>
        public Login(EventPriority priority, LoginPayload payload, DateTime eventTime)
            : base(priority, payload)
        {
            EventGenerationTime = eventTime;
        }
    }
}