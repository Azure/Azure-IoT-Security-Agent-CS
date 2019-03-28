// <copyright file="RemoteConfigurationParsingException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.Exceptions
{
    /// <summary>
    /// represents an exceptional state caused by a remote configuration parsing error
    /// </summary>
    public class RemoteConfigurationParsingException : Exception
    {
        /// <summary>
        /// The properties that caused the exception
        /// </summary>
        public IList<string> MisconfiguredProperties { get; }

        /// <inheritdoc />
        public override string Message { get; }

        /// <summary>
        /// C-tor 
        /// </summary>
        /// <param name="misconfiguredProperties">The message</param>
        public RemoteConfigurationParsingException(IList<string> misconfiguredProperties)
        {
            MisconfiguredProperties = misconfiguredProperties;
            Message = $"Couldn't parse the following configurations from the remote configuration: {string.Join(", ", misconfiguredProperties)}";
        }
    }
}
