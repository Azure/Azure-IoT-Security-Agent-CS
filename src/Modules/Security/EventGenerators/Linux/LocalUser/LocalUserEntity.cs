// <copyright file="LocalUserEntity.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.LocalUser
{
    /// <summary>
    /// Local user (OS registerd user) details
    /// </summary>
    public class LocalUserEntity
    {
        /// <summary>
        /// User ID
        /// </summary>
        public uint UserId { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Collection of Groups the user belongs to
        /// </summary>
        public List<Group> Groups { get; set; }
    }
}