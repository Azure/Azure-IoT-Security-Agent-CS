// <copyright file="MisconfigurationException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Exceptions
{
    /// <summary>
    /// represents an exceptional state caused by a misconfiguration of the agent
    /// </summary>
    public class MisconfigurationException : Exception
    {
        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The original exception</param>
        public MisconfigurationException(string message, Exception innerException): base(message, innerException)
        {
        }

        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="message">The message</param>
        public MisconfigurationException(string message) : base(message)
        {
        }
    }
}
