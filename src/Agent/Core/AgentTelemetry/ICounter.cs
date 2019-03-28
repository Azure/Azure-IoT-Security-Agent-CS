// <copyright file="ICounter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.AgentTelemetry
{
    /// <summary>
    /// Counter interface
    /// </summary>
    public interface ICounter
    {
        /// <summary>
        /// Increase count by 1
        /// </summary>
        void Increment();

        /// <summary>
        /// Increase count by the given amount (can be negative)
        /// </summary>
        /// <param name="amount">the amount to increase</param>
        void IncrementBy(int amount);
    }
}
