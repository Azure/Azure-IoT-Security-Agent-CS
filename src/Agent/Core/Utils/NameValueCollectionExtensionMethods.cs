// <copyright file="NameValueCollectionExtensionMethods.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// Extension methods for NameValueCollection
    /// Methods for value parsing
    /// </summary>
    public static class NameValueCollectionExtensionMethods
    {

        /// <summary>
        /// Gets the value of the given key as a TimeSpan value
        /// </summary>
        /// <param name="nameValueCollection">Key Value collection</param>
        /// <param name="key">the key to parse</param>
        /// <exception cref="ArgumentNullException">In case the given key is null or empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">In case the value can not be parsed</exception>
        /// <returns>TimeSpan value of the given key</returns>
        public static TimeSpan GetTimeSpanValueThrowOnFail(this NameValueCollection nameValueCollection, string key)
        {
            string value = GetStringValueThrowOnFail(nameValueCollection, key);
            if (TimeSpan.TryParse(value, out var result))
            {
                return result;
            }

            throw new ArgumentOutOfRangeException(paramName: key);
        }

        /// <summary>
        /// Gets the value of the given key as a string value
        /// </summary>
        /// <param name="nameValueCollection">Key Value collection</param>
        /// <param name="key">the key to parse</param>
        /// <exception cref="ArgumentNullException">In case the given key is null or empty</exception>
        /// <returns>string value of the given key</returns>
        public static string GetStringValueThrowOnFail(this NameValueCollection nameValueCollection, string key)
        {
            string val = nameValueCollection[key];
            if (!string.IsNullOrWhiteSpace(val))
            {
                return val;
            }

            throw new ArgumentNullException(paramName: key);
        }

        /// <summary>
        /// Gets the value of the given key as a Guid value
        /// </summary>
        /// <param name="nameValueCollection">Key Value collection</param>
        /// <param name="key">the key to parse</param>
        /// <exception cref="ArgumentNullException">In case the given key is null or empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">In case the value can not be parsed</exception>
        /// <returns>Guid value of the given key</returns>
        public static Guid GetGuidValueThrowOnFail(this NameValueCollection nameValueCollection, string key)
        {
            string value = GetStringValueThrowOnFail(nameValueCollection, key);
            if (Guid.TryParse(value, out var result))
            {
                return result;
            }

            throw new ArgumentOutOfRangeException(paramName: key);
        }

        /// <summary>
        /// Gets the value of the given key as a double value
        /// </summary>
        /// <param name="nameValueCollection">Key Value collection</param>
        /// <param name="key">the key to parse</param>
        /// <exception cref="ArgumentNullException">In case the given key is null or empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">In case the value can not be parsed</exception>
        /// <returns>double value of the given key</returns>
        public static double GetDoubleValueThrowOnFail(this NameValueCollection nameValueCollection, string key)
        {
            string value = GetStringValueThrowOnFail(nameValueCollection, key);
            if (double.TryParse(value, out var result))
            {
                return result;
            }

            throw new ArgumentOutOfRangeException(paramName: key);
        }

        /// <summary>
        /// Gets the value of the given key as a Enum value
        /// </summary>
        /// <param name="nameValueCollection">Key Value collection</param>
        /// <param name="key">the key to parse</param>
        /// <exception cref="ArgumentNullException">In case the given key is null or empty</exception>
        /// <exception cref="ArgumentOutOfRangeException">In case the value can not be parsed</exception>
        /// <returns>Enum value of the given key</returns>
        public static T GetEnumValueThrowOnFail<T>(this NameValueCollection nameValueCollection, string key) where T : struct
        {
            string value = GetStringValueThrowOnFail(nameValueCollection, key);
            if (Enum.TryParse(value, out T result))
            {
                return result;
            }

            throw new ArgumentOutOfRangeException(paramName: key);
        }
    }
}
