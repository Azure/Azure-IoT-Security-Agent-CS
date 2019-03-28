// <copyright file="ListeningPortsPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Represents all open ports (socket state=LISTEN) with local IP (who listens) and target (who can connect)
    /// </summary>
    public class ListeningPortsPayload : Payload
    {
        /// <summary>
        /// The protocol tcp or udp
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// The local address that is listening 
        /// </summary>
        public string LocalAddress { get; set; }
        /// <summary>
        /// The local port that is listening 
        /// </summary>
        public string LocalPort { get; set; }
        /// <summary>
        /// The remote address who the listener listens to
        /// </summary>
        public string RemoteAddress { get; set; }
        /// <summary>
        /// The remote port 
        /// </summary>
        public string RemotePort { get; set; }
    }
}
