// <copyright file="ConfigurationError.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>


using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents
{
    /// <summary>
    /// Configuration error event
    /// </summary>
    public class ConfigurationError : OperationalEvent<ConfigurationErrorPayload>
    {
        /// <inheritdoc />
        public ConfigurationError(ConfigurationErrorPayload[] payload) : base(payload)
        {
        }

        /// <inheritdoc />
        public override EventCategory Category { get; } = EventCategory.Triggered;
    }
}