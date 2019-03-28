// <copyright file="MessageStatisticsPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads
{
    /// <summary>
    /// Message statistic payload
    /// </summary>
    public class MessageStatisticsPayload : Payload
    {
        /// <summary>
        /// Total amount of messages sent
        /// </summary>
        public int MessagesSent { get; set; }

        /// <summary>
        /// Total amount of messages under 4kb 
        /// </summary>
        public int MessagesUnder4KB { get; set; }

        /// <summary>
        /// Total amount of failed messages
        /// </summary>
        public int TotalFailed { get; set; }
    }
}
