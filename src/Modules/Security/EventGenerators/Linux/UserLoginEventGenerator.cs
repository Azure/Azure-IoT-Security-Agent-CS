// <copyright file="UserLoginEventGenerator.cs" company="Microsoft">
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

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// User login event generator
    /// </summary>
    public class UserLoginEventGenerator : AuditEventGeneratorBase
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<Login>();

        /// <inheritdoc />
        /// user login events are collected by default, no config necessary
        protected override IEnumerable<string> AuditRulesPrerequisites => new List<string>();

        /// <inheritdoc />
        protected override IEnumerable<AuditEventType> AuditEventTypes => new List<AuditEventType> { AuditEventType.UserAuth, AuditEventType.UserLogin };

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public UserLoginEventGenerator() : this(ProcessUtil.Instance) { }

        /// <inheritdoc />
        public UserLoginEventGenerator(IProcessUtil processUtil) : base(processUtil)
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<IEvent> GetEventsImpl(IEnumerable<AuditEvent> auditEvents)
        {
            return auditEvents.Select(AuditEventToDeviceEvent).ToList();
        }

        /// <summary>
        /// Converts an audit event to a device event
        /// </summary>
        /// <param name="auditEvent">The audit event to be converted</param>
        /// <returns>Device event based on the input</returns>
        private IEvent AuditEventToDeviceEvent(AuditEvent auditEvent)
        {
            string remoteAddress = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Address, throwIfNotExist: false);
            LoginPayload payload = new LoginPayload
            {
                Executable = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Executable),
                ProcessId = Convert.ToUInt32(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ProcessId)),
                UserId = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.LoginUserId, throwIfNotExist: false),
                UserName = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Account, throwIfNotExist: false),
                Result = GetAuditEventLoginResult(auditEvent),
                RemoteAddress = remoteAddress == "?" ? null : remoteAddress,
                Operation = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Operation, throwIfNotExist: false)
            };

            return new Login(Priority, payload, auditEvent.TimeUTC);
        }

        private LoginPayload.LoginResult GetAuditEventLoginResult(AuditEvent auditEvent)
        {
            string loginResultAuditProperty = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Result);

            return loginResultAuditProperty == "success"
                    ? LoginPayload.LoginResult.Success
                    : LoginPayload.LoginResult.Fail;
        }
    }
}