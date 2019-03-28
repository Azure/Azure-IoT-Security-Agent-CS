// <copyright file="AgentMessage.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.MessageWorker
{
    /// <summary>
    /// Represents an agent message that will be sent to the hub
    /// </summary>
    public class AgentMessage
    {
        /// <summary>
        /// The version of the client
        /// </summary>
        public string AgentVersion { get; }

        /// <summary>
        /// The Id of the agent that is sending the message
        /// </summary>
        public Guid AgentId { get; }

        /// <summary>
        /// The version of the message schema that this message implements
        /// </summary>
        public string MessageSchemaVersion { get; } = "1.0";

        /// <summary>
        /// The list of events to put in the message body
        /// </summary>
        public IEnumerable<IEvent> Events { get; }

        /// <summary>
        /// Ctor - creates a new agent message object
        /// </summary>
        /// <param name="events">The list of events to put in the message body</param>
        /// <param name="agentId">The Id of the agent that is sending the message</param>
        /// <param name="agentVersion">The version of the client</param>
        public AgentMessage(IEnumerable<IEvent> events, Guid agentId, string agentVersion)
        {
            AgentId = agentId;
            AgentVersion = agentVersion;
            Events = new List<IEvent>(events);
        }

        /// <summary>
        /// Compiles the message body buffer from the events
        /// </summary>
        /// <returns>The message body text (JSON)</returns>
        public string GetMessageBodyBuffer()
        {
            return JsonConvert.SerializeObject(this, JsonFormatting.SerializationSettings);
        }

        /// <summary>
        /// Retruns Indented Json string that represents the entire message
        /// Note: use for debug purposes
        /// </summary>
        /// <returns>The message body indented text (JSON)</returns>
        public string GetPrintableMessage()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
