// <copyright file="Diagnostic.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class Diagnostic : TriggeredEvent<DiagnosticPayload>
    {
        /// <inheritdoc />
        public override EventType EventType { get; } = EventType.Diagnostic;
        
        /// <inheritdoc />
        public Diagnostic(EventPriority priority, DiagnosticPayload payload)
            : base(priority, payload)
        {
        }
    }
}