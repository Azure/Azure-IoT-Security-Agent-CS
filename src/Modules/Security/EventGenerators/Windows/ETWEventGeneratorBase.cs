// <copyright file="ETWEventGeneratorBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using static Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils.SystemEventUtils;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// A base class for event generators the rely on ETW data
    /// </summary>
    public abstract class ETWEventGeneratorBase : EventGenerator
    {
        /// <summary>
        /// A list of command that must be run as a prerequisite for this event
        /// </summary>
        protected abstract IEnumerable<string> PrerequisiteCommands { get; }

        /// <summary>
        /// A list of ETW event types that this generator handles
        /// </summary>
        protected abstract IEnumerable<ETWEventType> ETWEvents { get; }

        /// <summary>
        /// defines a delegate the implementing class must implement to convert specific ETW events to IEvent
        /// </summary>
        /// <param name="etwEvent">the ETW event to convert</param>
        /// <returns>an intance of IEvent corresponding to the given ETW event</returns>
        protected delegate IEvent EtwToIEventConverter(Dictionary<string, string> etwEvent);

        /// <summary>
        /// Maps a type of ETW event to the specific function that converts it to an IoT event
        /// </summary>
        protected abstract Dictionary<ETWEventType, EtwToIEventConverter> EtwToIotEventConverters { get; }

        private readonly Dictionary<ETWEventType, string> _lastRetrievedEventTimeStamps = new Dictionary<ETWEventType, string>();        

        private readonly IWmiUtils _wmiUtils;

        /// <inheritdoc />
        public ETWEventGeneratorBase(IProcessUtil processUtil, IWmiUtils wmiUtils)
        {
            _wmiUtils = wmiUtils;

            foreach (ETWEventType etwEvent in ETWEvents)
            {
                _lastRetrievedEventTimeStamps[etwEvent] = ManagementDateTimeConverter.ToDmtfDateTime(DateTime.Now);
            }

            foreach (string command in PrerequisiteCommands)
            {
                processUtil.ExecuteWindowsCommand(command);   
            }
        }

        /// <inheritdoc />
        public override IEnumerable<IEvent> GetEvents()
        {
            List<IEvent> events = new List<IEvent>();

            foreach (ETWEventType etwEvent in ETWEvents)
            {
                var etwEvents = _wmiUtils.RunWmiQuery(
                $"SELECT Message,TimeGenerated FROM Win32_NTLogEvent Where Logfile = 'Security' AND Eventcode = '{(int)etwEvent}' AND {TimeGeneratedFieldName} > '{_lastRetrievedEventTimeStamps[etwEvent]}'",
                TimeGeneratedFieldName,
                MessageFieldName);

                if (etwEvents.Any())
                {
                    _lastRetrievedEventTimeStamps[etwEvent] = etwEvents.First()[TimeGeneratedFieldName];
                }

                events.AddRange(etwEvents.Select( ev => EtwToIotEventConverters[etwEvent](ev)).Where(ev => ev != null));
            }

            return events;
        }
    }
}
