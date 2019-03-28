// <copyright file="ConnectionsPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Open connection event
    /// Represnts a connection from the device to teh internet
    /// We currently know to handle connection from family INET
    /// Connections from other families are required more investigation - for those connection we send an event with the raw
    /// data of the HexSaddr to be further investigated by the server
    /// </summary>
    public class ConnectionsPayload : Payload
    {
        /// <summary>
        /// Marks inbound or outbound connections
        /// </summary>
        public enum ConnectionDirection
        {
#pragma warning disable CS1591
            In,
            Out
#pragma warning restore CS1591
        }

        /// <summary>
        /// The full path of the process executable
        /// </summary>
        public string Executable { get; set; }

        /// <summary>
        /// The connection process Id
        /// </summary>
        public uint ProcessId { get; set; }

        /// <summary>
        /// The process who created the connection (including arguments)
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// The user id who performed the connection
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The remote address of the connection
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        /// The remote port of the connection
        /// </summary>
        public string RemotePort { get; set; }

        /// <summary>
        /// The local address of the connection
        /// </summary>
        public string LocalAddress { get; set; }

        /// <summary>
        /// The local port of the connection
        /// </summary>
        public string LocalPort { get; set; }

        /// <summary>
        /// The protocol of the connection
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// The result of the login attempt
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConnectionDirection Direction { get; set; }
    }
}