// <copyright file="SingleThreadedAgent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.Providers;
using Microsoft.Azure.IoT.Agent.Core.Scheduling;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Linq;

namespace Microsoft.Azure.IoT.Agent.Core
{
    /// <summary>
    /// A single threaded version of the agent harness
    /// </summary>
    public class SingleThreadedAgent : AgentBase
    {
        /// <summary>
        /// The task scheduler
        /// </summary>
        private readonly TaskScheduler _scheduler;

        /// <summary>
        /// Ctor - creates a new single threaded agent harness
        /// All of the tasks are scheduled on the same scheduler
        /// </summary>
        public SingleThreadedAgent()
        {
            _scheduler = new TaskScheduler(CancellationTokenSource.Token);
        }

        /// <inheritdoc />
        protected override void DoOnStart()
        {
            SetupEventGenerators(_scheduler);
            MessageBuilder messageBuilder = new MessageBuilder();
            _scheduler.AddTask(messageBuilder, LocalConfiguration.General.ConsumerInterval, DateTime.UtcNow);

            // This will take over the current running thread - this flag can be changed if we want this thread to detach and execute something else
            _scheduler.Start(runInCurrentThread: true);
        }
    }
}