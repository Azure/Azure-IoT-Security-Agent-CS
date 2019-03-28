// <copyright file="TimeUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils
{
    /// <summary>
    /// Helper class for DateTime objects
    /// </summary>
    public static class TimeUtils
    {
        /// <summary>
        /// Unix epoch time
        /// </summary>
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Get a DateTime object from a unix time string
        /// </summary>
        /// <param name="unixTime">A string representing unix time</param>
        /// <returns>DateTime object</returns>
        public static DateTime FromUnixTime(string unixTime)
        {
            return FromUnixTime(double.Parse(unixTime));
        }

        /// <summary>
        /// Get a DateTime object from a unix time double
        /// </summary>
        /// <param name="unixTime">Number representing unix time</param>
        /// <returns>DateTime object</returns>
        public static DateTime FromUnixTime(double unixTime)
        {
            return Epoch.AddSeconds(unixTime);
        }
    }
}