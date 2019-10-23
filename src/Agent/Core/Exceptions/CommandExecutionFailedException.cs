// <copyright file="CommandExecutionFailedException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Exceptions
{
    /// <summary>
    /// Represents an exceptional state when trying to execute a command
    /// </summary>
    public class CommandExecutionFailedException : Exception
    {
        /// <summary>
        /// The command that was executed
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// The error output corresponding to the failure
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// The exit code of the command
        /// </summary>
        public int ExitCode { get; }

        /// <inheritdoc />
        public override string Message => $"the command [{Command}] exited with code [{ExitCode}] and an error of [{Error}]";

        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="command">The command that caused the error</param>
        /// <param name="exitCode">Process exit code</param>
        /// <param name="error">Error message</param>
        public CommandExecutionFailedException(string command, int exitCode, string error)
        {
            Command = command;
            ExitCode = exitCode;
            Error = error;
        }
    }
}