// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.Linux
{
    /// <summary>
    /// The agent main entry point
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main function of the agent
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static void Main(string[] args)
        {
            //using (MultiThreadedAgent agent = new MultiThreadedAgent())
            using (SingleThreadedAgent agent = new SingleThreadedAgent())
            {
                agent.Start();
            }
        }
    }
}
