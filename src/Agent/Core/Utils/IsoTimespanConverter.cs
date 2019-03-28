// <copyright file="IsoTimespanConverter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using System;
using System.Xml;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// <see cref="JsonConverter"/> for <see cref="TimeSpan"/> in ISO8601 format
    /// Reference: https://github.com/JamesNK/Newtonsoft.Json/issues/863
    /// </summary>
    public class IsoTimespanConverter : JsonConverter
    {
        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }

        /// <inheritdoc />
        public override bool CanRead => true;
        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(TimeSpan))
                throw new ArgumentException();

            var spanString = reader.Value as string;
            if (spanString == null)
                return null;
            return XmlConvert.ToTimeSpan(spanString);
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var duration = (TimeSpan)value;
            writer.WriteValue(XmlConvert.ToString(duration));
        }
    }
}
