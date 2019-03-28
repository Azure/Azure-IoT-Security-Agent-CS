// <copyright file="Program.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core;
using System;

namespace Microsoft.Azure.Security.IoT.Agent.Linux
{
    class Program
    {
        static void Main(string[] args)
        {
            using (SingleThreadedAgent agent = new SingleThreadedAgent())
            {
                agent.Start();
            }
        }
    }
}
