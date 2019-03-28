// <copyright file="BaselinePayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// An OS Baseline payload, contains information about a failed baseline rule
    /// </summary>
    public class BaselinePayload : Payload
    {
        /// <summary>
        /// The decription of the rule
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The cce id of the failed rule
        /// </summary>
        public string CceId { get; set; }

        /// <summary>
        /// The result of the rule, Fail if rule failed, Err if there was an error while trying to run the rule
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// The error message
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// The severity of the problem
        /// </summary>
        public string Severity { get; set; }
    }
}