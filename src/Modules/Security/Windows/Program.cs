// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Windows;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.Windows
{
    public class Program
    {
        static void Main(string[] args)
        {
            WindowsAgentExecutionMode executionMode = args.Contains(CommandLineArguments.StandaloneMode) ? WindowsAgentExecutionMode.Standalone : WindowsAgentExecutionMode.Service;

            var runner = new WindowsAgentRunner();
            runner.Run(executionMode, "Azure Security of Things Agent");
        }
    }
}
