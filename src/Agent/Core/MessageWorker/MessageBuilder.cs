// <copyright file="MessageBuilder.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.AgentTelemetry;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients;
using Microsoft.Azure.IoT.Agent.Core.Scheduling;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.Core.MessageWorker
{
    /// <summary>
    /// Consumes events from the shared event queue on schedule and builds messages
    /// to be sent to the hub
    /// </summary>
    public class MessageBuilder : ITask
    {
        /// <summary>
        /// The last time a high priority event was sent
        /// </summary>
        private DateTime _lastHighPrioritySendTime;

        /// <summary>
        /// The last time a low priority event was sent
        /// </summary>
        private DateTime _lastLowPrioritySendTime;

        private const int SmallMessageSizeInBytes = 8 * 1024;

        /// <summary>
        /// The task interface method for executing the message builder work on schedule
        /// </summary>
        public void ExecuteTask()
        {
            BuildMessages();
        }

        /// <summary>
        /// Build messages from events in the queue and send them to the hub
        /// </summary>
        private void BuildMessages()
        {
            uint availableSize = EventQueueManager.Instance.AvailableDataSize();
            TimeSpan timeSinceLastHighMessage = DateTime.UtcNow - _lastHighPrioritySendTime;
            TimeSpan timeSinceLastLowMessage = DateTime.UtcNow - _lastLowPrioritySendTime;

            var maxMessageSize = AgentConfiguration.RemoteConfiguration.MaxMessageSize;
            // If we got to the max message size in the queue, we are sending the message as high priority even if there
            // weren't any actual high priority events
            if (availableSize >= maxMessageSize)
            {
                SimpleLogger.Debug("Max size reached: " + availableSize);
                // TODO: consider adding mechanism that will track and limit amount of open tasks
                Task task = DequeueMessagesAndSendAsync(EventPriority.High, maxMessageSize);
                _lastHighPrioritySendTime = DateTime.UtcNow;
            }

            if (timeSinceLastHighMessage >= AgentConfiguration.RemoteConfiguration.HighPriorityMessageFrequency)
            {
                SimpleLogger.Debug("Reached high priority send time");
                Task taskHigh = DequeueMessagesAndSendAsync(EventPriority.High, maxMessageSize);
                _lastHighPrioritySendTime = DateTime.UtcNow;
            }

            if (timeSinceLastLowMessage >= AgentConfiguration.RemoteConfiguration.LowPriorityMessageFrequency)
            {
                SimpleLogger.Debug("Reached low priority send time");
                Task taskLow = DequeueMessagesAndSendAsync(EventPriority.Low, maxMessageSize);
                _lastLowPrioritySendTime = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Dequeue messages from the queue with queuePriority and sends them asynchronously
        /// </summary>
        /// <param name="queuePriority">Queue priority to dequeue from</param>
        /// <param name="maxMessageSize">Max allowed message size to send</param>
        /// <returns>Awaitable send message task</returns>
        private async Task DequeueMessagesAndSendAsync(EventPriority queuePriority, uint maxMessageSize)
        {
            uint size = EventQueueManager.Instance.DequeueEventsFromMultipleQueues(queuePriority, maxMessageSize, out var eventsToSend);

            if (eventsToSend != null && eventsToSend.Count > 0)
            {
                SimpleLogger.Debug($"Sending message ({size})...");
                if (size < SmallMessageSizeInBytes)
                {
                    CounterType.MessagesUnder4KB.Get().Increment();
                }

                await SendMessage(eventsToSend, queuePriority);
            }
        }

        /// <summary>
        /// Send the message asynchronously
        /// </summary>
        /// <param name="eventsToSend">The events to send in the message</param>
        /// <param name="messagePriority">Message priority</param>
        /// <returns>The awaitable task returned by the SDK</returns>
        private async Task SendMessage(IEnumerable<IEvent> eventsToSend, EventPriority messagePriority)
        {
            var context = ThreadContext.Get();
            AgentMessage message = new AgentMessage(eventsToSend,
                LocalConfiguration.General.AgentId,
                LocalConfiguration.AgentVersion);
            try
            {
                await ExternalInterfaceFacade.Instance.ExternalClient.SendMessage(message,
                    new Dictionary<string, string>
                    {
                        { "MessagePriority", messagePriority.ToString() }
                    });
                ThreadContext.Set(context);
                SimpleLogger.Debug("Message sent: " + message.GetPrintableMessage());
                CounterType.SendSuccesfully.Get().Increment();
            }
            catch (Exception e)
            {
                ThreadContext.Set(context);
                SimpleLogger.Warning("Message failed to sent: " + message.GetPrintableMessage(), false, e);
                CounterType.SendFailed.Get().Increment();
            }
        }
    }
}