// <copyright file="LocalUsersParser.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.LocalUser
{
    /// <summary>
    /// Helper class for parsing /etc/passwd and /etc/groups
    /// </summary>
    public static class LocalUsersParser
    {
        #region Consts

        private const string FieldsDelimiter = ":";
        private const string ValuesDelimiter = ",";
        private const int UsersFileUsernameCol = 0;
        private const int UsersFileUidCol = 2;
        private const int UsersFileGidCol = 3;
        private const int GroupsFileGroupNameCol = 0;
        private const int GroupsFileGidCol = 2;
        private const int GroupsFileUsersCol = 3;

        #endregion

        /// <summary>
        /// Gets the content of /etc/passwd and /etc/group, and creates a LocalUser list from the files' content
        /// </summary>
        /// <param name="groupFileContent">The content of /etc/group file</param>
        /// <param name="passwdFileContent">The content of /etc/passwd file</param>
        /// <returns>List of local users based on the content</returns>
        public static List<LocalUserEntity> ListAllLocalUsersFromContent(string groupFileContent,
            string passwdFileContent)
        {
            var gidToGroup = CreateGidToGroupNameDictFromContent(groupFileContent);
            var usersToNonDefaultGroupsId = CreateUsersToNonDefaultGroupsIdDictFromContent(groupFileContent);

            return passwdFileContent
                .SplitStringOnNewLine()
                .Select(line => GenereateLocalUserEntityFromPasswdLine(line, gidToGroup, usersToNonDefaultGroupsId))
                .ToList();
        }

        /// <summary>
        /// Parse the content of /etc/group and maps GroupId => GroupNames
        /// </summary>
        /// <param name="groupsFileContent">The content of /etc/group file</param>
        /// <returns>Group id to group names dictionary</returns>
        private static IDictionary<uint, string> CreateGidToGroupNameDictFromContent(string groupsFileContent)
        {
            var gidToGroupNameDict = new Dictionary<uint, string>();
            string[] lines = groupsFileContent.SplitStringOnNewLine();

            foreach (string line in lines)
            {
                var parts = line.Split(FieldsDelimiter);
                uint gid = uint.Parse(parts[GroupsFileGidCol]);
                string groupName = parts[GroupsFileGroupNameCol];

                string newValue = groupName;

                if (gidToGroupNameDict.TryGetValue(gid, out string groupNames))
                {
                    newValue = $"{groupNames}{LocalUsersSnapshotGenerator.SchemaDelimiter}{groupName}";
                }

                gidToGroupNameDict[gid] = newValue;
            }

            return gidToGroupNameDict;
        }

        /// <summary>
        /// Parse the content of /etc/group and maps users to their groups - not including the users' default group
        /// A user's default group is the group that linux creates when creating the user
        /// </summary>
        /// <param name="content">The content of /etc/group file</param>
        /// <returns>Users to groups dictionary</returns>
        private static IDictionary<string, List<uint>> CreateUsersToNonDefaultGroupsIdDictFromContent(string content)
        {
            var usersToNonDefaultGroupsId = new Dictionary<string, List<uint>>();

            foreach (var line in content.SplitStringOnNewLine())
            {
                var values = line.Split(FieldsDelimiter);

                if (!string.IsNullOrEmpty(values[GroupsFileUsersCol]))
                {
                    var users = values[GroupsFileUsersCol].Split(ValuesDelimiter);
                    var currentGroupId = uint.Parse(values[GroupsFileGidCol]);
                    foreach (var user in users)
                        if (usersToNonDefaultGroupsId.ContainsKey(user))
                        {
                            usersToNonDefaultGroupsId[user].Add(currentGroupId);
                        }
                        else
                        {
                            usersToNonDefaultGroupsId[user] = new List<uint> { currentGroupId };
                        }
                }
            }

            return usersToNonDefaultGroupsId;
        }

        /// <summary>
        /// Parse an entry from /etc/passwd and generates a LocalUsersEntity from the given entry
        /// </summary>
        /// <param name="userLine">A line entry from /etc/passwd</param>
        /// <param name="gidToGroup">Group id to group name dictionary</param>
        /// <param name="usersToNonDefaultGroupsId">User to groups dictionary</param>
        /// <returns>LocalUserEntity</returns>
        private static LocalUserEntity GenereateLocalUserEntityFromPasswdLine(string userLine,
            IDictionary<uint, string> gidToGroup, IDictionary<string, List<uint>> usersToNonDefaultGroupsId)
        {
            var properties = userLine.Split(FieldsDelimiter);

            var userName = properties[UsersFileUsernameCol];
            var userId = uint.Parse(properties[UsersFileUidCol]);
            var defaultGroup = uint.Parse(properties[UsersFileGidCol]);
            var groupIds = new List<uint> { defaultGroup };

            if (usersToNonDefaultGroupsId.ContainsKey(userName))
            {
                groupIds.AddRange(usersToNonDefaultGroupsId[userName]);
            }

            return new LocalUserEntity
            {
                Username = userName,
                UserId = userId,
                Groups = groupIds.Select(group => new Group
                {
                    Name = gidToGroup[group],
                    Id = group
                }).ToList()
            };
        }
    }
}