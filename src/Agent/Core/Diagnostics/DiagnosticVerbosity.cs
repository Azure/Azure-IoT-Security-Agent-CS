// <copyright file="DiagnosticVerbosity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.Diagnostics
{
    /// <summary>
    /// The verbosity of diagnostic events 
    /// </summary>
    public enum DiagnosticVerbosity
    {
        /// <summary>
        /// None - no diagnostic events will be sent
        /// </summary>
        None,
        /// <summary>
        /// Some - only some (important) logs will be sent as diagnostic events
        /// </summary>
        Some,
        /// <summary>
        /// All - all logs will be sent as diagnostic events, regardless of log verbosity
        /// </summary>
        All
    }
}
