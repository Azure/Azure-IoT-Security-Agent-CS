// <copyright file="IWmiUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils
{
    /// <summary>
    /// A utility for fetching data on windows machines using WMI
    /// </summary>
    public interface IWmiUtils
    {
        /// <summary>
        /// Get a single value from WMI
        /// </summary>
        /// <param name="type">The type of data to retrieve</param>
        /// <returns>The value</returns>
        string GetSingleValue(WmiDataType type);

        /// <summary>
        /// Get a list of values from WMI
        /// </summary>
        /// <param name="type">The type of data to retrieve</param>
        /// <returns>The list of values</returns>
        IEnumerable<string> GetEnumerableValue(WmiDataType type);

        /// <summary>
        /// Get a result from a WMI query
        /// </summary>
        /// <param name="query">The query to run</param>
        /// <param name="values">The values to extract from the result of the query</param>
        /// <returns>Populated key value map</returns>
        IEnumerable<Dictionary<string, string>> RunWmiQuery(string query, params string[] values);
    }
}