// <copyright file="LocalUsersSnapshotGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.LocalUser;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Snapshot generator for registered OS users on the device.
    /// </summary>
    public class LocalUsersSnapshotGenerator : SnapshotEventGenerator
    {
        /// <summary>
        /// Schema plural values delimiter
        /// </summary>
        public static readonly char SchemaDelimiter = ';';

        private readonly IProcessUtil _processUtil;

        #region Consts - Shell Commands

        private const string GetUsersListCommand = "cat /etc/passwd";
        private const string GetGroupListCommand = "cat /etc/group";

        #endregion

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<LocalUsers>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public LocalUsersSnapshotGenerator() : this(ProcessUtil.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public LocalUsersSnapshotGenerator(IProcessUtil processUtil)
        {
            _processUtil = processUtil;
        }

        /// <summary>
        /// Generates a LocalUser snapshot
        /// The event payload is array of OS registered users,
        /// each user has its own payload entity which contains
        /// the user's username, userid, group ids and group names
        /// </summary>
        /// <returns>List of local users snapshot event, the list should contain only one element</returns>
        protected override List<IEvent> GetEventsImpl()
        {
            string groupsFileContent = _processUtil.ExecuteBashShellCommand(GetGroupListCommand);
            string passwdFileContent = _processUtil.ExecuteBashShellCommand(GetUsersListCommand);

            var users = LocalUsersParser.ListAllLocalUsersFromContent(groupsFileContent, passwdFileContent);

            var localUsersPayloads = GeneratePayloadsFromLocalUsers(users);
            SimpleLogger.Debug($"BaselineEventGenerator returns {localUsersPayloads.Count()} payloads");
            return new List<IEvent>
            {
                new LocalUsers(
                    priority: Priority,
                    payloads:localUsersPayloads )
            };
        }
        #region Private Methods

        /// <summary>
        /// Gets a LocalUserEntity collection and generates a LocalUserPayload from the given collection
        /// </summary>
        /// <param name="users">User entity collection</param>
        /// <returns>The payloads</returns>
        private LocalUsersPayload[] GeneratePayloadsFromLocalUsers(IEnumerable<LocalUserEntity> users)
        {
            return users
                .Select(user => new LocalUsersPayload
                {
                    UserId = user.UserId.ToString(),
                    UserName = user.Username,
                    GroupIds = string.Join(SchemaDelimiter, user.Groups.Select(group => group.Id)),
                    GroupNames = string.Join(SchemaDelimiter, user.Groups.Select(groups => groups.Name))
                })
                .ToArray();
        }

        #endregion
    }
}