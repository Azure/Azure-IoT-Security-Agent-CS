// <copyright file="AgentFailedToStartException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using System;

namespace Microsoft.Azure.IoT.Agent.Core.Exceptions
{
    class AgentFailedToStartException : Exception
    {
        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The original exception</param>
        public AgentFailedToStartException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
