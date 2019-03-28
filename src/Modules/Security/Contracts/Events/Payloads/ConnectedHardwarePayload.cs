// <copyright file="ConnectedHardwarePayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Connected hardware payload
    /// </summary>
    public class ConnectedHardwarePayload : Payload
    {
        /// <summary>
        /// A connected hardware device
        /// </summary>
        public string ConnectedHardware { get; set; }
    }
}