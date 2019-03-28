// <copyright file="MultiThreadedAgent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.Providers;
using Microsoft.Azure.IoT.Agent.Core.Scheduling;
using System.Threading;
using System.Linq;
using Microsoft.Azure.IoT.Contracts.Events;
using System;

namespace Microsoft.Azure.IoT.Agent.Core
{
    /// <summary>
    /// A multi threaded version of the agent harness
    /// </summary>
    public class MultiThreadedAgent : AgentBase
    {
        /// <summary>
        /// The task scheduler for the message builder
        /// </summary>
        private readonly TaskScheduler _consumerScheduler;

        /// <summary>
        /// The task scheduler for the event producer
        /// </summary>
        private readonly TaskScheduler _producerScheduler;

        /// <summary>
        /// Ctor - creates a new agent harness object
        /// </summary>
        public MultiThreadedAgent()
        {
            _producerScheduler = new TaskScheduler(CancellationTokenSource.Token);
            _consumerScheduler = new TaskScheduler(CancellationTokenSource.Token);
        }

        /// <inheritdoc />
        protected override void DoOnStart()
        {
            SetupEventGenerators(_producerScheduler);
            
            MessageBuilder messageBuilder = new MessageBuilder();
            _consumerScheduler.AddTask(messageBuilder, LocalConfiguration.General.ConsumerInterval, DateTime.UtcNow);

            _producerScheduler.Start();
            _consumerScheduler.Start();

            // TODO: keep alive properly
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(5000);
            }
        }
    }
}