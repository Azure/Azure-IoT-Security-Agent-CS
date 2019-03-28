// <copyright file="WindowsAgentRunner.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Windows;
using System;
using System.ServiceProcess;

namespace Microsoft.Azure.IoT.Agent.Windows
{
    /// <summary>
    /// Entry point for starting the agent on a Windows environment. Either as a service or a standalone process
    /// </summary>
    public class WindowsAgentRunner
    {
        /// <summary>
        /// Starts running the agent
        /// </summary>
        /// <param name="exeuctionMode">The execution mode in which the agent should operate</param>
        /// <param name="serviceName">The name of the service</param>
        public void Run(WindowsAgentExecutionMode exeuctionMode, string serviceName)
        {
            switch (exeuctionMode)
            {
                case WindowsAgentExecutionMode.Standalone:
                    {
                        new AgentService(serviceName).OnDebug();
                        break;
                    }
                case WindowsAgentExecutionMode.Service:
                    {
                        var serviceToRun = new ServiceBase[] { new AgentService(serviceName) };
                        ServiceBase.Run(serviceToRun);
                        break;
                    }
                default:
                    throw new ArgumentException("Unknown execution mode", nameof(exeuctionMode));
            }
        }
    }
}
