// <copyright file="ProcessCreationPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Process creation payload
    /// </summary>
    public class ProcessCreationPayload : Payload
    {
        /// <summary>
        /// The full path of the process executable
        /// </summary>
        public string Executable { get; set; }

        /// <summary>
        /// The process Id
        /// </summary>
        public uint ProcessId { get; set; }

        /// <summary>
        /// The process Id of the parent
        /// </summary>
        public uint ParentProcessId { get; set; }

        /// <summary>
        /// The name of the account name under which the process was started
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The name of the account id under which the process was started
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The command line used to start the process, includes the executable path and arguments
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// The executable hash
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// The The time the event happened
        /// </summary>
        [JsonIgnore]
        public DateTime Time { get; set; }
    }
}