// <copyright file="ExceptionCodes.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.Core.Exceptions
{
    /// <summary>
    /// Agent exception codes and subcodes.
    /// </summary>
    public enum ExceptionCodes
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Description("Local Configuration")]
        LocalConfiguration,
        [Description("Remote Configuration")]
        RemoteConfiguration,
        [Description("Authentication")]
        Authentication
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
    
    /// <summary>
    /// Exception sub codes
    /// </summary>
    public enum ExceptionSubCodes
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Description("Missing Configuration")]
        MissingConfiguration,
        [Description("Cant Parse Configuration")]
        CantParseConfiguration,
        [Description("Timeout")]
        Timeout,
        [Description("File Format")]
        FileFormat,
        [Description("File Not Exist")]
        FileNotExist,
        [Description("File Permissions")]
        FilePermissions,
        [Description("Unauthorized")]
        Unauthorized,
        [Description("Not Found")]
        NotFound,
        [Description("Other")]
        Other
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Extension methods for agent exceptions
    /// </summary>
    public static class ExceptionCodesExtension
    {
        /// <summary>
        /// Gets description of error code
        /// </summary>
        /// <param name="error">the error code</param>
        /// <returns>error description</returns>
        public static string GetDescription(this ExceptionCodes error)
        {
            return GetDescriptionFromAttribute(error);
        }

        /// <summary>
        /// Gets description of error sub code
        /// </summary>
        /// <param name="error">the error sub code</param>
        /// <returns>error descriptio</returns>
        public static string GetDescription(this ExceptionSubCodes error)
        {
            return GetDescriptionFromAttribute(error);
        }

        private static string GetDescriptionFromAttribute(this Enum error)
        {
            return error.GetType()
                .GetMember(error.ToString())
                .First()
                .GetCustomAttribute<DescriptionAttribute>()
                .Description;
        }
    }
}