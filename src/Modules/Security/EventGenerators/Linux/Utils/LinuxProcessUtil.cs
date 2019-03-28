// <copyright file="LinuxProcessUtil.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Microsoft.Azure.IoT.Agent.Core.Utils;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils
{
    /// <summary>
    /// Helper class for executing bash shell commands
    /// </summary>
    public static class LinuxProcessUtil
    {
        /// <summary>
        /// Execute shell (bin/bash) commands
        /// </summary>
        /// <param name="processUtil">process utility instnace</param>
        /// <param name="cmd">Command to execute</param>
        /// <param name="errorHandler">Error handler to be called in case of an error while executing the command</param>
        /// <param name="acceptableExitCodes">Exit codes that considerd as OK</param>
        /// <returns>Command output</returns>
        public static string ExecuteBashShellCommand(this IProcessUtil processUtil, string cmd, ErrorHandler errorHandler = null, IEnumerable<int> acceptableExitCodes = null)
        {
            string args = $"-c \"{cmd}\"";
            return processUtil.ExecuteProcess("/bin/bash", args, errorHandler, acceptableExitCodes);
        }
    }
}