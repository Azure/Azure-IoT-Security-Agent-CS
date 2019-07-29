// <copyright file="AggregatedConnectionCreateEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.Common.AggregatedEventGenerators;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Aggregated connection create event generator 
    /// </summary>
    public class AggregatedConnectionCreateEventGenerator : AggregatedConnectionCreateEventGeneratorCommon
    {
        /// <summary>
        /// C-tor
        /// </summary>
        public AggregatedConnectionCreateEventGenerator() : base(new ConnectionEventGenerator()) { }
    }
}
