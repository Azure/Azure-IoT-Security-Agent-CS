// <copyright file="CounterType.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.AgentTelemetry
{
    /// <summary>
    /// Types of agent performance counters
    /// </summary>
    public enum CounterType
    {
        /// <summary>
        /// Messages small than 4Kb
        /// </summary>
        MessagesUnder4KB,
        /// <summary>
        /// Dropped high priority events
        /// </summary>
        DroppedHighPriorityEvent,
        /// <summary>
        /// Enqueued high priority events
        /// </summary>
        EnqueuedHighPriorityEvent,
        /// <summary>
        /// Dropped low priority events
        /// </summary>
        DroppedLowPriorityEvent,
        /// <summary>
        /// Enqueued low priority events
        /// </summary>
        EnqueuedLowPriorityEvent,
        /// <summary>
        /// Dropped operational events
        /// </summary>
        DroppedOperationalEvent,
        /// <summary>
        /// Enqueued operational events
        /// </summary>
        EnqueuedOperationalEvent,
        /// <summary>
        /// Sent succesfully messages
        /// </summary>
        SendSuccesfully,
        /// <summary>
        /// Failed to send messages
        /// </summary>
        SendFailed
    }
}
