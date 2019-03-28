// <copyright file="LocalusersEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Snapshot generator for registered OS users on the device.
    /// </summary>
    public class LocalusersEventGenerator : SnapshotEventGenerator
    {
        private string NameKey = "Name";
        private string SidKey = "Sid";
        private readonly IWmiUtils _wmiUtils;
        private readonly IProcessUtil _processUtil;

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<LocalUsers>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil and WmiUtil
        /// </summary>
        public LocalusersEventGenerator() : this (WmiUtils.Instance, ProcessUtil.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public LocalusersEventGenerator(IWmiUtils wmiUtils, IProcessUtil processUtil)
        {
            _wmiUtils = wmiUtils;
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
            var users = _wmiUtils.RunWmiQuery("SELECT Name,Sid FROM Win32_UserAccount Where LocalAccount = True", NameKey, SidKey);
            var groups = _wmiUtils.RunWmiQuery("SELECT Name,Sid FROM Win32_Group Where LocalAccount = True", NameKey, SidKey);

            IEnumerable<LocalUsersPayload> localUsersPayloads = users.Select(user => GetUserPayload(user, groups));

            return new List<IEvent>
            {
                new LocalUsers(
                    priority: Priority,
                    payloads:localUsersPayloads.ToArray() )
            };
        }

        private LocalUsersPayload GetUserPayload(Dictionary<string,string> user, IEnumerable<Dictionary<string, string>> groups)
        {
            LocalUsersPayload payload = new LocalUsersPayload();

            payload.UserName = user[NameKey];
            payload.UserId = user[SidKey];

            var userGroups = FilterGroupsByUser(payload.UserName, groups);

            payload.GroupNames = string.Join(',', userGroups.Select(group => group[NameKey]));
            payload.GroupIds = string.Join(',', userGroups.Select(group => group[SidKey]));

            return payload;   
        }

        private IEnumerable<Dictionary<string, string>> FilterGroupsByUser(string userName, IEnumerable<Dictionary<string, string>> groups)
        {
            string netUserProps = _processUtil.ExecuteWindowsCommand($"net user {userName}");
            string spaceDelimitedGroups = Regex.Match(netUserProps, @"(?<=Local Group Memberships\s+)[^\n]+").Value;
            IEnumerable<string> groupStrings = Regex.Split(spaceDelimitedGroups, @"\s+").Select(group => group.Trim('*'));

            return groups.Where(group => groupStrings.Contains(group[NameKey]));
        }
    }
}
