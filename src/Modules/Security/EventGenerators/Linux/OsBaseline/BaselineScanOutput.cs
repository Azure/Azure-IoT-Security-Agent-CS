// <copyright file="BaselineScanOutput.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.OsBaseline
{
    /// <summary>
    /// Represent the entire baseline scan output
    /// </summary>
    public class BaselineScanOutput
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [JsonProperty(PropertyName = "baseline_id")]
        public string BaselineId { get; set; }

        [JsonProperty(PropertyName = "base_orig_id")]
        public string BaseOriginId { get; set; }

        [JsonProperty(PropertyName = "setting_count")]
        public int SettingCount { get; set; }

        [JsonProperty(PropertyName = "scan_time")]
        public DateTime ScanTime { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "results")]
        public List<BaselineResult> Results { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}