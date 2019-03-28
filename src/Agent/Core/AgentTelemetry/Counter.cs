// <copyright file="Counter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Threading;

namespace Microsoft.Azure.IoT.Agent.Core.AgentTelemetry
{
    /// <summary>
    /// Thread safe counter
    /// </summary>
    class Counter : ICountProvider, ICounter
    {
        private int _count;

        /// <inheritdoc />
        public void Increment()
        {
            IncrementBy(1);
        }

        /// <inheritdoc />
        public void IncrementBy(int amount)
        {
            Interlocked.Add(ref _count, amount);
        }

        /// <inheritdoc />
        public int GetCountAndReset()
        {
            return Interlocked.Exchange(ref _count, 0);
        }
    }
}
