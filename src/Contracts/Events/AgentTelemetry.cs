// <copyright file="AgentTelemetry.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Agent telemetry event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AgentTelemetry<T> : OperationalEvent<T>
        where T : Payload, new()
    {
        /// <inheritdoc />
        public override EventCategory Category => EventCategory.Periodic;

        /// <inheritdoc />
        protected AgentTelemetry(T[] payload) : base(payload)
        {
        }
    }
}
