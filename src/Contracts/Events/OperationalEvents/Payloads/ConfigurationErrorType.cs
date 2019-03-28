// <copyright file="ConfigurationErrorType.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads
{
    /// <summary>
    /// Error in agent twin's configuration
    /// </summary>
    public enum ConfigurationErrorType
    {
        /// <summary>
        /// The value type does not match configuration type
        /// </summary>
        TypeMismatch,

        /// <summary>
        /// Configuration is not optimal
        /// </summary>
        NotOptimal,

        /// <summary>
        /// Configuration conflicts with another configuration
        /// </summary>
        Conflict
    }
}