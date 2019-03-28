// <copyright file="LocalUsersPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <inheritdoc />
    public class LocalUsersPayload : Payload
    {
        /// <summary>
        /// Username 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The name of the groups that the user is a member of
        /// </summary>
        public string GroupNames { get; set; }

        /// <summary>
        /// The id of the groups that the user is a member of
        /// </summary>
        public string GroupIds { get; set; }
    }
}