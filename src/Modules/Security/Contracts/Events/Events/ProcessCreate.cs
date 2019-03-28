// <copyright file="ProcessCreate.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class ProcessCreate : SecurityTriggeredEvent<ProcessCreationPayload>
    {
        /// <inheritdoc />
        public override DateTime EventGenerationTime => Payload.First().Time;

        /// <inheritdoc />
        public ProcessCreate(EventPriority priority, ProcessCreationPayload payload)
            : base(priority, payload)
        {
        }
    }
}