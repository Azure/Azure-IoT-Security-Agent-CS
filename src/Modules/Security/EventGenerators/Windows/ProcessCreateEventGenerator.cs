// <copyright file="ProcessCreateEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using static Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils.SystemEventUtils;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Event generator for windows process create
    /// </summary>
    public class ProcessCreateEventGenerator : ETWEventGeneratorBase
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ProcessCreate>();

        /// <inheritdoc />
        protected override IEnumerable<string> PrerequisiteCommands => new[] { "auditpol /set /subcategory:\"Process Creation\" /Success:enable /Failure:enable" };

        /// <inheritdoc />
        protected override IEnumerable<ETWEventType> ETWEvents => new[] { ETWEventType.ProcessCreate };

        /// <inheritdoc />
        protected override Dictionary<ETWEventType, EtwToIEventConverter> EtwToIotEventConverters => new Dictionary<ETWEventType, EtwToIEventConverter>()
        {
            { ETWEventType.ProcessCreate, GetProcessCreationEvent }
        };

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default WmiUtils and ProcessUtil
        /// </summary>
        public ProcessCreateEventGenerator() : this(ProcessUtil.Instance, WmiUtils.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public ProcessCreateEventGenerator(IProcessUtil processUtil, IWmiUtils wmiUtils) : base( processUtil, wmiUtils)
        {
        }

        private IEvent GetProcessCreationEvent(Dictionary<string, string> ev)
        {
            var commandline = GetEventPropertyFromMessage(ev[MessageFieldName], NewProcessCommandLineFieldName);
            var executable = GetEventPropertyFromMessage(ev[MessageFieldName], ProcessNameFieldName);

            var payload = new ProcessCreationPayload
            {
                Executable = executable,
                ProcessId = Convert.ToUInt32(GetEventPropertyFromMessage(ev[MessageFieldName], ProcessIdFieldName), 16),
                ParentProcessId = Convert.ToUInt32(GetEventPropertyFromMessage(ev[MessageFieldName], CreatorProcessIdFieldName), 16),
                UserName = GetEventPropertyFromMessage(ev[MessageFieldName], AccountNameFieldName, 1),
                UserId = GetEventPropertyFromMessage(ev[MessageFieldName], LogonIdFieldName),
                CommandLine = string.IsNullOrWhiteSpace(commandline) ? executable : commandline,
                Time = DateTime.Parse(ev[TimeGeneratedFieldName]),
                ExtraDetails = new Dictionary<string, string>
                {
                    { $"CREATOR_{SecurityIdFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], SecurityIdFieldName) },
                    { $"CREATOR_{AccountDomainFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], AccountDomainFieldName) },
                    { $"CREATOR_{AccountNameFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], AccountNameFieldName) },
                    { $"TARGET_{SecurityIdFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], SecurityIdFieldName, 1) },
                    { $"TARGET_{AccountDomainFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], AccountDomainFieldName, 1) },
                    { $"TARGET_{AccountNameFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], AccountNameFieldName, 1) },
                    { $"TARGET_{LogonIdFieldName}", GetEventPropertyFromMessage(ev[MessageFieldName], LogonIdFieldName, 1) },
                    { TokenElevationTypeFieldName, GetEventPropertyFromMessage(ev[MessageFieldName], TokenElevationTypeFieldName) },
                    { MandatoryLabelFieldName, GetEventPropertyFromMessage(ev[MessageFieldName], MandatoryLabelFieldName) }
                }
            };

            return new ProcessCreate(AgentConfiguration.GetEventPriority<ProcessCreate>(), payload);
        }
    }
}