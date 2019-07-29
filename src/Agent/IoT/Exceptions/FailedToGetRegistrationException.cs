// <copyright file="FailedToGetModuleException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using System;

namespace Microsoft.Azure.IoT.Agent.IoT.Exceptions
{
    /// <summary>
    /// Exception indicates that a Registration object cannot be retrieved
    /// </summary>
    public class FailedToGetRegistrationException : Exception
    {
        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="message">The message</param>
        public FailedToGetRegistrationException(string message) : base(message)
        {
        }
    }
}