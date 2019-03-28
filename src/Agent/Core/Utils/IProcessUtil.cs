// <copyright file="IProcessUtil.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// Error handling result
    /// </summary>
    public enum ErrorHandlerResult
    {
        /// <summary>
        /// Error is handled, do not continue invoking error handlers
        /// </summary>
        ErrorHandled,
        /// <summary>
        /// Error is not handled, invoke the next error handler
        /// </summary>
        ErrorNotHandled
    }

    /// <summary>
    /// Delegate for error handling while executing a command
    /// If the error is not handled, control passes to the DefaultErrorHandler and throws an error
    /// </summary>
    /// <param name="errorCode">Process exit code</param>
    /// <param name="errorMessage">Error output</param>
    /// <param name="command">The command that caused the error</param>
    /// <returns>Handler result</returns>
    public delegate ErrorHandlerResult ErrorHandler(int errorCode, string errorMessage, string command);

    /// <summary>
    /// Helper for executing commands on a linux machine
    /// </summary>
    public interface IProcessUtil
    {
        /// <summary>
        /// Execute process with its arguments.
        /// Note - to execute shell commands use the following:
        /// Linux - Agent.EventGenerators.Linux.Utils.LinuxProcessUtil.ExecuteShellCommand
        /// For Windows programName="cmd.exe" and arguments should start with "/C"
        /// </summary>
        /// <param name="programName">Program to execute</param>
        /// <param name="args">Program arguments</param>
        /// <param name="errorHandler">A handler for process errors</param>
        /// <param name="acceptableExitCodesOverride">Exit codes that considerd as OK</param>
        /// <returns>Command output</returns>
        string ExecuteProcess(string programName, string args, ErrorHandler errorHandler, IEnumerable<int> acceptableExitCodesOverride);
    }
}