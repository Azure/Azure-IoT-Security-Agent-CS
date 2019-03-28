// <copyright file="ProcessTerminate.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class ProcessTerminate : SecurityTriggeredEvent<ProcessTerminationPayload>
    {
        /// <inheritdoc />
        public override DateTime EventGenerationTime => Payload.First().Time;

        /// <inheritdoc />
        public ProcessTerminate(EventPriority priority, ProcessTerminationPayload payload)
            : base(priority, payload)
        {
        }
    }
}