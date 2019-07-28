// <copyright file="ConfigurationUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// Configuration related utilities
    /// </summary>
    public static class ConfigurationUtils
    {
        /// <summary>
        /// Appends the given path to the current process's path
        /// </summary>
        /// <param name="path">Relative path</param>
        /// <returns>The combined path</returns>
        public static string AppendToProcessPath(string path)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string processDirectory = Path.GetDirectoryName(executingAssembly.Location);
            string combined = Path.Combine(processDirectory, path);

            return combined;
        }

        /// <remarks>
        /// Should be replaced with Path.IsPathFullyQualified (introduced in .NET Core 2.1)
        /// https://docs.microsoft.com/en-us/dotnet/api/system.io.path.ispathfullyqualified?view=netcore-2.1
        /// </remarks>
        public static bool IsFullyQualifiedPath(string path)
        {
            string fullPath = Path.GetFullPath(path);
            bool result = fullPath.Equals(path);

            return result;
        }
    }
}
