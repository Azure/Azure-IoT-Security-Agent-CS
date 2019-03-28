// <copyright file="WindowsProcessUtil.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Utils;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils
{
    /// <summary>
    /// Helper class for executing bash shell commands
    /// </summary>
    public static class WindowsProcessUtil
    {
        /// <summary>
        /// Execute shell (bin/bash) commands
        /// </summary>
        /// <param name="processUtil">process utility instnace</param>
        /// <param name="cmd">Command to execute</param>
        /// <param name="errorHandler">Error handler to be called in case of an error while executing the command</param>
        /// <param name="acceptableExitCodes">Exit codes that considerd as OK</param>
        /// <returns>Command output</returns>
        public static string ExecuteWindowsCommand(this IProcessUtil processUtil, string cmd, ErrorHandler errorHandler = null, IEnumerable<int> acceptableExitCodes = null)
        {
            string args = $"/C \"{cmd}\"";
            return processUtil.ExecuteProcess(@"C:\Windows\System32\cmd.exe", args, errorHandler, acceptableExitCodes);
        }

        /// <summary>
        /// Execute powershell commands
        /// </summary>
        /// <param name="processUtil">process utility instnace</param>
        /// <param name="cmd">Command to execute</param>
        /// <param name="errorHandler">Error handler to be called in case of an error while executing the command</param>
        /// <param name="acceptableExitCodes">Exit codes that considerd as OK</param>
        /// <returns>Command output</returns>
        public static string ExecutePowershellCommand(this IProcessUtil processUtil, string cmd, ErrorHandler errorHandler = null, IEnumerable<int> acceptableExitCodes = null)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            string args = $"-Command \"[System.Threading.Thread]::CurrentThread.CurrentCulture = [cultureinfo]::GetCultureInfo('{currentCulture}');{cmd}\"";
            return processUtil.ExecuteProcess(@"PowerShell", args, errorHandler, acceptableExitCodes);
        }
    }
}