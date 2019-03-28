// <copyright file="Group.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.LocalUser
{
    /// <summary>
    /// Linux group details
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Linux group Id
        /// </summary>
        public uint Id { get; set; }

        /// <summary>
        /// Linux group name
        /// </summary>
        public string Name { get; set; }
    }
}