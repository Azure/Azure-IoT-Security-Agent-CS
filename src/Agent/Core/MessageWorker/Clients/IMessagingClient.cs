// <copyright file="IMessagingClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients
{
    /// <summary>
    /// Represents a client to the external interface
    /// </summary>
    public interface IMessagingClient : IDisposable
    {
        /// <summary>
        /// Sends an <see cref="AgentMessage"/> through the external interface
        /// </summary>
        /// <param name="messageBody">The message to send</param>
        /// <param name="messageProperties">Additional message properties</param>
        /// <returns></returns>
        Task SendMessage(AgentMessage messageBody, Dictionary<string, string> messageProperties);
    }
}
