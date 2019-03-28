// <copyright file="ProcessTerminationPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Process exit payload
    /// </summary>
    public class ProcessTerminationPayload : Payload
    {
        /// <summary>
        /// The full path of the process image
        /// </summary>
        public string Executable { get; set; }

        /// <summary>
        /// The process Id
        /// </summary>
        public uint ProcessId { get; set; }

        /// <summary>
        /// The exit status of the process
        /// </summary>
        public int ExitStatus { get; set; }

        /// <summary>
        /// The The time the event happened
        /// </summary>
        [JsonIgnore]
        public DateTime Time { get; set; }
    }
}