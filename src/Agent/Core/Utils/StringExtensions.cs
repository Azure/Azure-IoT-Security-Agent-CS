// <copyright file="StringExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Splits the given string on new lines
        /// </summary>
        /// <param name="str">String to split</param>
        /// <returns>Array of strings, each string is a line from the given string</returns>
        public static string[] SplitStringOnNewLine(this string str)
        {
            return str.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}