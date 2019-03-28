// <copyright file="ProcessCreationEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Process creation event generator for Linux
    /// </summary>
    public class ProcessCreationEventGenerator : AuditEventGeneratorBase
    {
        /// <inheritdoc />
        protected override IEnumerable<string> AuditRulesPrerequisites => new[]
        {
            "-a always,exit -F arch=b32 -S execve,execveat",
            "-a always,exit -F arch=b64 -S execve,execveat"
        };

        /// <inheritdoc />
        protected override IEnumerable<AuditEventType> AuditEventTypes => new[] { AuditEventType.ProcessExecution };

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ProcessCreate>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public ProcessCreationEventGenerator() : this(ProcessUtil.Instance) { }

        /// <inheritdoc />
        public ProcessCreationEventGenerator(IProcessUtil processUtil) : base(processUtil)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<IEvent> GetEventsImpl(IEnumerable<AuditEvent> auditEvents)
        {
            return auditEvents.Select(AuditEventToDeviceEvent);
        }

        /// <summary>
        /// Converts an audit event to a device event
        /// </summary>
        /// <param name="auditEvent">Audit event to convert</param>
        /// <returns>Device event based on the input</returns>
        private IEvent AuditEventToDeviceEvent(AuditEvent auditEvent)
        {
            ProcessCreationPayload payload = new ProcessCreationPayload
            {
                CommandLine = EncodedAuditFieldsUtils.DecodeHexStringIfNeeded(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.CommandLine), Encoding.UTF8),
                Executable = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Executable),
                ProcessId = Convert.ToUInt32(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ProcessId)),
                ParentProcessId = Convert.ToUInt32(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ParentProcessId)),
                Time = auditEvent.TimeUTC,
                UserId = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.UserId)
            };

            return new ProcessCreate(Priority, payload);
        }
    }
}