// <copyright file="ProcessUtil.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <inheritdoc />
    public class ProcessUtil : IProcessUtil
    {
        private const int SuccessExitCode = 0;

        private static readonly IEnumerable<int> DeafultAcceptableExitCodes = new[] { SuccessExitCode };

        private static readonly Lazy<ProcessUtil> _instance = new Lazy<ProcessUtil>();

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ProcessUtil Instance => _instance.Value;


        /// <inheritdoc />
        public string ExecuteProcess(string programName, string args, ErrorHandler errorHandler,
            IEnumerable<int> acceptableExitCodesOverride)
        {
            using (Process processToExecute = new Process())
            {
                processToExecute.StartInfo.FileName = programName;
                processToExecute.StartInfo.Arguments = args;
                processToExecute.StartInfo.RedirectStandardOutput = true;
                processToExecute.StartInfo.RedirectStandardError = true;
                processToExecute.StartInfo.UseShellExecute = false;
                processToExecute.StartInfo.CreateNoWindow = true;
                processToExecute.Start();

                string result = processToExecute.StandardOutput.ReadToEnd();
                string error = processToExecute.StandardError.ReadToEnd();

                processToExecute.WaitForExit();

                int exitCode = processToExecute.ExitCode;
                if (!(acceptableExitCodesOverride ?? DeafultAcceptableExitCodes).Contains(exitCode))
                {
                    RunErrorHandler(exitCode, error, $"{programName} {args}", errorHandler);
                }

                return result;
            }
        }

        private void RunErrorHandler(int exitCode, string error, string cmd, ErrorHandler errorHandler = null)
        {
            ErrorHandlerResult handlerResult = errorHandler?.Invoke(exitCode, error, cmd) ?? ErrorHandlerResult.ErrorNotHandled;

            if (handlerResult == ErrorHandlerResult.ErrorNotHandled)
            {
                SimpleLogger.Warning($"Process exited with error: {cmd}, exitcode={exitCode}, message={error}");
                throw new CommandExecutionFailedException(cmd, exitCode, error);
            }
        }
    }
}