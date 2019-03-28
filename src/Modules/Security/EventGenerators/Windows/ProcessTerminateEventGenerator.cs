// <copyright file="ProcessTerminateEventGenerator.cs" company="Microsoft">
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
    /// Event generator for windows process exit
    /// </summary>
    public class ProcessTerminateEventGenerator : ETWEventGeneratorBase
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ProcessTerminate>();

        /// <inheritdoc />
        protected override IEnumerable<string> PrerequisiteCommands => new[] { "auditpol /set /subcategory:\"Process Termination\" /Success:enable /Failure:enable" };

        /// <inheritdoc />
        protected override IEnumerable<ETWEventType> ETWEvents => new[] { ETWEventType.ProcessExit };

        /// <inheritdoc />
        protected override Dictionary<ETWEventType, EtwToIEventConverter> EtwToIotEventConverters => new Dictionary<ETWEventType, EtwToIEventConverter>()
        {
            { ETWEventType.ProcessExit, GetProcessExitEvent }
        };

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default WmiUtils and ProcessUtil
        /// </summary>
        public ProcessTerminateEventGenerator() : this(ProcessUtil.Instance, WmiUtils.Instance) { }

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public ProcessTerminateEventGenerator(IProcessUtil processUtil, IWmiUtils wmiUtils) : base(processUtil, wmiUtils)
        {
        }

        private IEvent GetProcessExitEvent(Dictionary<string, string> ev)
        {
            var payload = new ProcessTerminationPayload
            {
                Executable = GetEventPropertyFromMessage(ev[MessageFieldName], ProcessNameFieldName),
                ProcessId = Convert.ToUInt32(GetEventPropertyFromMessage(ev[MessageFieldName], ProcessIdFieldName), 16),
                ExitStatus = Convert.ToInt32(GetEventPropertyFromMessage(ev[MessageFieldName], ExitStatusFieldName), 16),
                Time = DateTime.Parse(ev[TimeGeneratedFieldName]),
                ExtraDetails = new Dictionary<string, string>
                {
                    { AccountDomainFieldName, GetEventPropertyFromMessage(ev[MessageFieldName], AccountDomainFieldName) },
                    { AccountNameFieldName, GetEventPropertyFromMessage(ev[MessageFieldName], AccountNameFieldName) },
                    { LogonIdFieldName, GetEventPropertyFromMessage(ev[MessageFieldName], LogonIdFieldName) }
                }
            };

            return new ProcessTerminate(AgentConfiguration.GetEventPriority<ProcessTerminate>(), payload);
        }
    }
}