// <copyright file="ExceptionExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// Exception extension methods
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Format an exception and a message into one string
        /// </summary>
        /// <param name="exception">The exception</param>
        /// <param name="message">The message</param>
        /// <returns></returns>
        public static string FormatExceptionMessage(this Exception exception, string message)
        {
            return $"{message} : {exception}";
        }
    }
}
