// <copyright file="RemoteConfigurationPnPConverter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration
{
    /// <summary>
    /// Converts remote configuration properties
    /// </summary>
    public class RemoteConfigurationPnPConverter : JsonConverter
    {
        private const string ValueField = "value";
        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject configObject = new JObject
            {
                new JProperty(ValueField, value)
            };

            serializer.Serialize(writer, configObject);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject config = JObject.Load(reader);
                JToken value = config[ValueField];
                if (value != null)
                {
                    return value.ToObject(objectType, serializer);
                }
            }

            throw new ArgumentException("Remote configuration does not match pnp schema");
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
