// <copyright file="ConfigurationUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using System;

namespace Microsoft.Azure.Security.IoT.Agent.Common.Utils
{
    /// <summary>
    /// Configuration related utilities
    /// </summary>
    public static class ConfigurationUtils
    {
        /// <summary>
        /// Read a symmetric key string from a given file
        /// </summary>
        /// <param name="filePath">file path for the symmetric key</param>
        /// <returns>Symmetric key as it was provided in the File Path</returns>
        /// <exception cref="MisconfigurationException">In case the file is empty</exception>
        public static string GetSymmetricKeyFromFile(string filePath)
        {
            //Read the connection string from the file:
            string content = System.IO.File.ReadAllText(filePath);
            char[] charsToTrim = Environment.NewLine.ToCharArray();
            char[] whiteSpace = { ' ' };
            string key = content.Trim(charsToTrim).Trim(whiteSpace);
            if (string.IsNullOrEmpty(key))
            {
                throw new MisconfigurationException($"Could not read key from file: {filePath}");
            }
            return key;
        }
    }
}
