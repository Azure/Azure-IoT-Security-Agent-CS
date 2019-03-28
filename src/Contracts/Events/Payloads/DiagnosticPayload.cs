// <copyright file="DiagnosticPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Contracts.Events.Payloads
{
    /// <inheritdoc />
    public class DiagnosticPayload : Payload
    {
        /// <summary>
        /// Correlation Id for the diagonstic event
        /// </summary>
        public Guid CorrelationId { get; set; }
        
        /// <summary>
        /// The severity of the diagnostic event
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The Id of the process in which the event occured
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// The Id of the thread in which the event occured
        /// </summary>
        public int ThreadId { get; set; }
    }
}