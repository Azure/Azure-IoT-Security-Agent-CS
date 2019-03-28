// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Windows
{
    /// <summary>
    /// The agent main entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main function of the agent
        /// </summary>
        /// <param name="args">command line arguments</param>
        static void Main(string[] args)
        {
            WindowsAgentExecutionMode executionMode = args.Contains(CommandLineArguments.StandaloneMode) ? WindowsAgentExecutionMode.Standalone : WindowsAgentExecutionMode.Service;

            var runner = new WindowsAgentRunner();
            runner.Run(executionMode, "Azure IoT Agent");
        }
    }
}