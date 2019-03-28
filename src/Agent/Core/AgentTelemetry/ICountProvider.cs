// <copyright file="ICountProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.AgentTelemetry
{
    interface ICountProvider
    {
        /// <summary>
        /// Gets the counter data
        /// </summary>
        /// <returns>count</returns>
        int GetCountAndReset();
    }
}
