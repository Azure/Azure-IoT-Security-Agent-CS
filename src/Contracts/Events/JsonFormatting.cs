// <copyright file="JsonFormatting.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Newtonsoft.Json;
using System.Globalization;

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Json formatting settings class
    /// </summary>
    public static class JsonFormatting
    {
        /// <summary>
        /// Contains the json format settings according to our schema
        /// </summary>
        public static JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            Culture = CultureInfo.InvariantCulture,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "s" // 2008-04-10T06:30:00,
        };
    }
}