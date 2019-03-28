// <copyright file="WindowsAgentExecutionMode.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Windows
{
    /// <summary>
    /// Represents the agent's supported exeuction modes
    /// </summary>
    public enum WindowsAgentExecutionMode
    {
        /// <summary>
        /// The agent executes as a standalone process
        /// </summary>
        Standalone,

        /// <summary>
        /// The agent executes as a Windows Service
        /// </summary>
        Service
    }
}
