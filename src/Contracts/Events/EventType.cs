// <copyright file="EventType.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Represent the type of the event
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Security releated events
        /// </summary>
        Security,

        /// <summary>
        /// Operational events
        /// </summary>
        Operational,

        /// <summary>
        /// Diagnostice events (internal purposes)
        /// </summary>
        Diagnostic
    }
}
