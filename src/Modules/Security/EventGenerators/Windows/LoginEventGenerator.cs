// <copyright file="LoginEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using static Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils.SystemEventUtils;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.LoginPayload;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Event generator for windows login events
    /// </summary>
    public class LoginEventGenerator : ETWEventGeneratorBase
    {
        private const string FailureReasonExtraDetailsKey = "Failure Reason";
        private const string AccountDomainExtraDetailsKey = "Account Domain";

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<Login>();

        /// <inheritdoc /> //change to the correct one!!!
        protected override IEnumerable<string> PrerequisiteCommands => new List<string>();

        /// <inheritdoc />
        protected override Dictionary<ETWEventType, EtwToIEventConverter> EtwToIotEventConverters => new Dictionary<ETWEventType, EtwToIEventConverter>()
        {
            { ETWEventType.LoginSuccess, ConvertSystemEventToLoginSuccessEvent },
            { ETWEventType.LoginFailure, ConvertSystemEventToLoginFailureEvent }
        };

        /// <inheritdoc />
        protected override IEnumerable<ETWEventType> ETWEvents => new[] { ETWEventType.LoginSuccess, ETWEventType.LoginFailure };

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default WmiUtil
        /// </summary>
        public LoginEventGenerator() : this(WmiUtils.Instance, ProcessUtil.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public LoginEventGenerator(IWmiUtils wmiUtils, IProcessUtil processUtil) : base(processUtil, wmiUtils)
        {
        }

        private string GetSidFromUsername(string username)
        {
            try
            {
                NTAccount account = new NTAccount(username);
                SecurityIdentifier sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
                return sid.ToString();
            }
            catch (IdentityNotMappedException ex)
            {
                SimpleLogger.Warning($"Could not translate username {username} to sid", exception:ex);
                return string.Empty;
            }
        }

        private IEvent ConvertSystemEventToLoginSuccessEvent(Dictionary<string, string> loginEvent)
        {
            string userName = GetEventPropertyFromMessage(loginEvent[MessageFieldName], AccountNameFieldName, 1);
            string domain = GetEventPropertyFromMessage(loginEvent[MessageFieldName], AccountDomainFieldName, 1);
            return new Login(Priority, new LoginPayload()
            {
                ProcessId = Convert.ToUInt32(GetEventPropertyFromMessage(loginEvent[MessageFieldName], ProcessIdFieldName), 16),
                UserName = domain + @"\" + userName,
                UserId = GetSidFromUsername(userName),
                Operation = GetEventPropertyFromMessage(loginEvent[MessageFieldName], LogonProcessFieldName),
                Executable = GetEventPropertyFromMessage(loginEvent[MessageFieldName], ProcessNameFieldName),
                RemoteAddress = GetEventPropertyFromMessage(loginEvent[MessageFieldName], SourceNetworkAddressFieldName),
                RemotePort = GetEventPropertyFromMessage(loginEvent[MessageFieldName], SourcePortFieldName),
                Result = LoginResult.Success,
                ExtraDetails = new Dictionary<string, string>() { { AccountDomainExtraDetailsKey, domain } }
            }, DateTime.Parse(loginEvent[TimeGeneratedFieldName]));
        }

        private Login ConvertSystemEventToLoginFailureEvent(Dictionary<string, string> loginEvent)
        {
            string userName = GetEventPropertyFromMessage(loginEvent[MessageFieldName], AccountNameFieldName, 1);
            string domain = GetEventPropertyFromMessage(loginEvent[MessageFieldName], AccountDomainFieldName, 1);
            return new Login(Priority, new LoginPayload()
            {
                ProcessId = Convert.ToUInt32(GetEventPropertyFromMessage(loginEvent[MessageFieldName], CallerProcessIdFieldName), 16),
                UserName = domain + @"\" + userName,
                UserId = GetSidFromUsername(userName),
                Operation = GetEventPropertyFromMessage(loginEvent[MessageFieldName], LogonProcessFieldName),
                Executable = GetEventPropertyFromMessage(loginEvent[MessageFieldName], CallerProcessNameFieldName),
                RemoteAddress = GetEventPropertyFromMessage(loginEvent[MessageFieldName], SourceNetworkAddressFieldName),
                RemotePort = GetEventPropertyFromMessage(loginEvent[MessageFieldName], SourcePortFieldName),
                Result = LoginResult.Fail,
                ExtraDetails = new Dictionary<string,string>()
                {
                    { AccountDomainExtraDetailsKey, domain },
                    { FailureReasonExtraDetailsKey, GetEventPropertyFromMessage(loginEvent[MessageFieldName], FailureReasonFieldName) }
                }
            }, DateTime.Parse(loginEvent[TimeGeneratedFieldName]));
        }
    }
}
