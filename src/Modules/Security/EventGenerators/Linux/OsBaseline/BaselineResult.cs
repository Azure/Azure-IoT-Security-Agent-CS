// <copyright file="BaselineResult.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.OsBaseline
{
    /// <summary>
    /// Represent a single result of a rule in the baseline scan output
    /// </summary>
    public class BaselineResult
    {
        /// <summary>
        /// Baseline result types
        /// </summary>
        public enum ResultType
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            Pass,
            Fail,
            Err,
            Skip
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// Baseline result severity levels
        /// </summary>
        public enum SeverityLevel
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            Critical,
            Important,
            Warning,
            Informational
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// The MS ID of the rule, not used for anything on our side
        /// </summary>
        [JsonProperty(PropertyName = "msid")]
        public string MsId { get; set; }

        /// <summary>
        /// Result type
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public ResultType Result { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        [JsonProperty(PropertyName = "error_text")]
        public string ErrorText { get; set; }

        /// <summary>
        /// The Common TwinConfiguration Enumeration unique identifier of the rule
        /// </summary>
        [JsonProperty(PropertyName = "cceid")]
        public string CceId { get; set; }

        /// <summary>
        /// Result's severity
        /// </summary>
        [JsonProperty(PropertyName = "severity")]
        public SeverityLevel Severity { get; set; }

        /// <summary>
        /// Result's description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Rule id
        /// </summary>
        [JsonProperty(PropertyName = "ruleId")]
        public Guid RuleId { get; set; }
    }
}