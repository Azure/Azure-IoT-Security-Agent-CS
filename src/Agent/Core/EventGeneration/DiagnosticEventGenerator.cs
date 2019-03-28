// <copyright file="DiagnosticEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Events;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// Event generator for diagnostic events
    /// This event generator collects all system diagnostic events
    /// </summary>
    public class DiagnosticEventGenerator : EventGenerator
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<Diagnostic>();

        /// <summary>
        /// Internal buffer to hold diagnostic events
        /// </summary>
        private readonly EventQueue _internalBuffer = new EventQueue(QueueSizePercentage);

        /// <summary>
        ///  The fraction of size from the total amount of space allocated for the agent queues
        /// </summary>
        private const double QueueSizePercentage = 0.2;
        
        /// <summary>
        /// C-tor
        /// Registers to System DiagnosticEvent
        /// </summary>
        public DiagnosticEventGenerator()
        {
            SystemEvents.DiagnosticeEvent += CreateDiagnosticEvent;
        }

        /// <inheritdoc />
        public override IEnumerable<IEvent> GetEvents()
        {
            return DequeueAll();
        }

        private void CreateDiagnosticEvent(string message, LogLevel severity, Exception exception = null)
        {
            var payload = new DiagnosticPayload()
            {
                CorrelationId = ThreadContext.Get().ExecutionId,
                Severity = severity.ToString(),
                Message = exception == null ? message : exception.FormatExceptionMessage(message),
                ProcessId = Process.GetCurrentProcess().Id,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };

            _internalBuffer.Enqueue(new Diagnostic(Priority, payload)); 
        }

        private IEnumerable<IEvent> DequeueAll()
        {
            var returnedEvents = new List<IEvent>();

            var ev = _internalBuffer.Dequeue();
            while (ev != null)
            {
                returnedEvents.Add(ev);
                ev = _internalBuffer.Dequeue();
            }

            return returnedEvents;
        }
    }
}
