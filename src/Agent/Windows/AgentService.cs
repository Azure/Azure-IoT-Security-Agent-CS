// <copyright file="AgentService.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core;
using System.ServiceProcess;
using System.Threading;

namespace Microsoft.Azure.IoT.Agent.Windows
{
    public partial class AgentService : ServiceBase
    {
        private AgentBase _agent;

        /// <summary>
        /// ctor
        /// </summary>
        public AgentService(string serviceName)
        {
            InitializeComponent(serviceName);
        }

        /// <summary>
        /// Called when the project was built in debug configuration, this is for development purposes
        /// </summary>
        public void OnDebug()
        {
            RunAgent();
        }

        /// <summary>
        /// Called when the service is attempting to start, this method must bring up a new thread an return
        /// </summary>
        protected override void OnStart(string[] args)
        {
            Thread agentThread = new Thread(new ThreadStart(RunAgent));
            agentThread.Start();
        }

        /// <summary>
        /// Called when the service is attempting to stop, this must cleanup and return
        /// </summary>
        protected override void OnStop()
        {
            _agent.Dispose();
        }

        private void RunAgent()
        {
            //_agent = new MultiThreadedAgent();
            _agent = new SingleThreadedAgent();
            _agent.Start();
            Stop();
        }
    }
}
