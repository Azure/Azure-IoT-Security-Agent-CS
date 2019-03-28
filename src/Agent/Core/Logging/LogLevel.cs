// <copyright file="LogLevel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.Logging
{
    /// <summary>
    /// The log level of the agent or an individual log message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Used to turn off logging
        /// </summary>
        Off = 0,
        /// <summary>
        /// Fatal, messages with this level indicate an error that prevents the agent from running
        /// </summary>
        Fatal = 1,
        /// <summary>
        /// Error, messages with this level indicate a failure to do something
        /// </summary>
        Error = 2,
        /// <summary>
        /// Warning, messages with this level indicate a possibility for a problematic state
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Information, general information about the execution of the agent
        /// </summary>
        Information = 4,
        /// <summary>
        /// Debug, more verbose information about the execution of the agent
        /// </summary>
        Debug = 5
    }
}