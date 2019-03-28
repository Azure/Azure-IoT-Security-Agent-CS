// <copyright file="FirewallRulePayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <inheritdoc />
    public class FirewallRulePayload : Payload
    {
        /// <summary>
        /// Rule actions
        /// </summary>
        public enum Actions
        {
#pragma warning disable CS1591 
            Allow,
            Deny,
            Other
#pragma warning restore CS1591
        }

        /// <summary>
        /// Rule directions
        /// </summary>
        public enum Directions
        {
#pragma warning disable CS1591
            In,
            Out,
#pragma warning restore CS1591
        }

        /// <summary>
        /// Rule priority  
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// Rule direction
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Directions? Direction { get; set; }

        /// <summary>
        /// Is rule enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Rule action. Other is used for special cases, details should be provided under ExtraDetails.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public Actions Action { get; set; }

        /// <summary>
        /// The application the rule applies to
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Rule parent chain name
        /// </summary>
        public string ChainName { get; set; }

        /// <summary>
        /// Source IP address or tag
        /// </summary>
        public string SourceAddress { get; set; }

        /// <summary>
        /// Source Port
        /// </summary>
        public string SourcePort { get; set; }

        /// <summary>
        /// Destination IP address or tag
        /// </summary>
        public string DestinationAddress { get; set; }

        /// <summary>
        /// Remote Port
        /// </summary>
        public string DestinationPort { get; set; }

        /// <summary>
        /// Rule connection protocol
        /// </summary>
        public string Protocol { get; set; }
    }
}