// <copyright file="DiagnosticEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core
{
    /// <summary>
    /// System diagnostic event handler
    /// </summary>
    /// <param name="message">the diagnostic message</param>
    /// <param name="severity">event severiyt</param>
    /// <param name="exception">the exception releated to the diagnostic event</param>
    public delegate void DiagnosticEvent(string message, Logging.LogLevel severity, Exception exception = null);

    /// <summary>
    /// System events
    /// </summary>
    public static class SystemEvents
    {
        /// <summary>
        /// System diagnostic event
        /// </summary>
        public static event DiagnosticEvent DiagnosticeEvent;

        /// <summary>
        /// Dispatch a new diagnostic event
        /// </summary>
        /// <param name="message">the diagnostic message</param>
        /// <param name="severity">event severity</param>
        /// <param name="exception">the exception releated to the diagnostic event</param>
        public static void DispatchDiagnosicEvent(string message, Logging.LogLevel severity, Exception exception = null)
        {
            DiagnosticeEvent?.Invoke(message, severity, exception);
        }
    }
}
