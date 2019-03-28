// <copyright file="TelemetryCollector.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.AgentTelemetry
{
    /// <summary>
    /// Manages all agent telemetry counters
    /// </summary>
    public static class TelemetryCollector
    {
        private static readonly IDictionary<CounterType, Counter> Counters;
        private static readonly object Lock = new object();

        static TelemetryCollector()
        {
            Counters = new ConcurrentDictionary<CounterType, Counter>();
            foreach (CounterType counterType in Enum.GetValues(typeof(CounterType)))
            {
                Counters[counterType] = new Counter();
            }
        }

        /// <summary>
        /// Gets the counter of counter type
        /// </summary>
        /// <returns>The counter</returns>
        public static ICounter GetCounter(CounterType counterType)
        {
            return Counters[counterType];
        }

        /// <summary>
        /// Gets the data from all the counters and resets them to their default value
        /// </summary>
        /// <returns>A dictionary mapped with counter type to the counted value</returns>
        public static IDictionary<CounterType, int> GetDataAndReset()
        {
            return Counters.ToDictionary(
                key => key.Key,
                val => val.Value.GetCountAndReset());
        }

        /// <summary>
        /// Gets the corresponding counter
        /// </summary>
        /// <param name="counterType">The counter type to get</param>
        /// <returns>ICounter</returns>
        public static ICounter Get(this CounterType counterType)
        {
            return GetCounter(counterType);
        }
    }
}
