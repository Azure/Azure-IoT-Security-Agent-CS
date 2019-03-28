// <copyright file="FakeEventPayload .cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents
{
    /// <summary>
    /// Payload for all fake events
    /// </summary>
    public class FakeEventPayload : Payload
    {
        /// <summary>
        /// First parameter
        /// </summary>
        public string Param1 { get; set; }

        /// <summary>
        /// Second parameter
        /// </summary>
        public string Param2 { get; set; }

        /// <summary>
        /// Default value for testing
        /// </summary>
        [JsonIgnore] public static readonly FakeEventPayload DefaultFakeEventPayload = new FakeEventPayload
        {
            Param1 = "Test1",
            Param2 = "Test2"
        };
    }
}