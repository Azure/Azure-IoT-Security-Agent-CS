// <copyright file="JsonUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// Utilities to handle Json tokens
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// Remove keys in the Json object that has null values
        /// </summary>
        /// <param name="token"></param>
        /// <returns>the given JToken with no keys that have null values</returns>
        public static JToken RemoveKeysWithNullValue(JToken token)
        {
            JObject resObject = new JObject();
            if (token.Type == JTokenType.Object)
            {
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    JToken node = prop.Value;
                    if (node.HasValues)
                    {
                        node = RemoveKeysWithNullValue(node);
                    }
                    if (!IsEmpty(node))
                    {
                        resObject.Add(prop.Name, node);
                    }
                }
            }
            return resObject;
        }
        /// <summary>
        /// Check if Jtoken is null or empty
        /// </summary>
        /// <param name="token"></param>
        /// <returns>true if token is null or empty</returns>
        public static bool IsEmpty(JToken token)
        {
            if (token == null)
            {
                return true;
            }
            return (token.Type == JTokenType.Null);
        }

        /// <summary>
        /// Gets the json property name of the given property
        /// </summary>
        /// <param name="propertyName">class property</param>
        /// <param name="classType">the class the property belongs to</param>
        /// <returns>json property label</returns>
        public static string GetJsonLabel(string propertyName, Type classType)
        {
        var propertyInfo = classType.GetProperty(propertyName);
        var jAttr = (JsonPropertyAttribute)propertyInfo.GetCustomAttribute(typeof(JsonPropertyAttribute));
            return jAttr?.PropertyName ?? propertyInfo.Name;
        }
    }
}
