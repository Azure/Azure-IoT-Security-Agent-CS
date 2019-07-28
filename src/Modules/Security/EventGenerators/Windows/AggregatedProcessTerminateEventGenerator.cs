// <copyright file="AggregateProcessTerminateEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.Common.AggregatedEventGenerators;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// windows process terminate event aggregator
    /// </summary>
    public class AggregateProcessTerminateEventGenerator : AggregatedProcessTerminateEventGeneratorCommon
    {
        /// <summary>
        /// C-tor
        /// </summary>
        public AggregateProcessTerminateEventGenerator() : base(new ProcessTerminateEventGenerator()) { }

    }
}
