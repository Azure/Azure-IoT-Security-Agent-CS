// <copyright file="AggregatedProcessCreateEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.Common.AggregatedEventGenerators;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Aggregated process create event generator 
    /// </summary>
    public class AggregatedProcessCreateEventGenerator : AggregatedProcessCreateEventGeneratorCommon
    {
        /// <summary>
        /// C-tor
        /// </summary>
        public AggregatedProcessCreateEventGenerator() : base(new ProcessCreateEventGenerator()) { }
    }
}
