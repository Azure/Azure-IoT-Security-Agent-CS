// <copyright file="IPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Event payload interface
    /// </summary>
    public abstract class Payload
    {
        /// <summary>
        /// Extra details for non-trivial filtering
        /// </summary>
        public IDictionary<string, string> ExtraDetails { get; set; }
    }
}