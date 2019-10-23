// <copyright file="ConnectionsEventGeneratorBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Connections;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.ConnectionsPayload;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Collect all the tcp connections that were open from/to the machine to/from the internet
    /// The collection is done by reading the auditd logs for all "connect" or "accept" calls type with "success"
    /// </summary>
    public abstract class ConnectionsEventGeneratorBase : AuditEventGeneratorBase
    {
        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<ConnectionCreate>();

        /// <inheritdoc />
        protected override IEnumerable<AuditEventType> AuditEventTypes => new List<AuditEventType>
        {
            AuditEventType.ProcessTitle,
            AuditEventType.ConnectSockaddr
        };

        /// <inheritdoc />
        protected ConnectionsEventGeneratorBase(IProcessUtil processUtil) : base(processUtil)
        {
        }

        /// <summary>
        /// Inbound or Outbound connections should derive this class and set the correct direction
        /// </summary>
        /// <returns>Connection direction of generated events</returns>
        protected abstract ConnectionDirection GetConnectionDirection();

        /// <inheritdoc />
        protected override IEnumerable<IEvent> GetEventsImpl(IEnumerable<AuditEvent> auditEvents)
        {
            var returnList = new List<IEvent>();

            foreach (var auditEvent in auditEvents)
            {
                var item = CreateEventFromAuditRecord(auditEvent);
                if (item != null)
                {
                    returnList.Add(item);
                }
            }

            return returnList;
        }

        /// <summary>
        /// This function recieve an event from the audit log file
        /// It filters out connections that are not relevant for security (e.g. local connects)
        /// It then returns "ConnectionCreate" event type that represent a succefull open connection from/to the internet
        /// </summary>
        /// <param name="auditEvent">A log event from the the audit event</param>
        /// <returns>A device event based on the input</returns>
        private IEvent CreateEventFromAuditRecord(AuditEvent auditEvent)
        {
            ConnectionsPayload connectionPayload = null;
            ConnectionCreate retConnection = null;

            string saddr = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.SocketAddress, throwIfNotExist: false);

            if (!string.IsNullOrEmpty(saddr))
            {
                //Check the address family of the connection - extract from the saddr
                LinuxAddressFamily family = ConnectionSaddr.ExtractFamilyFromSaddr(saddr);

                //According to the family type we create/don't create the event
                if (!family.IsToIgnore()) //irelevant connections - don't create events
                {
                    if (ConnectionSaddr.IsInetFamliy(family)) //internet connections - create correlated event
                    {
                        connectionPayload = CreateInetConnPayloadFromAuditEvent(auditEvent);
                    }
                    else //For other famlies (non INET) that are required more investigation - send event with raw data (hex string)
                    {
                        connectionPayload = CreateNonInetConnPayloadFromAuditEvent(family, auditEvent);
                    }
                }
            }
            else
            {
                SimpleLogger.Debug($"{nameof(GetType)}: Saddr is null or empty, dropping event");
            }

            if (connectionPayload != null)
            {
                retConnection = new ConnectionCreate(Priority, connectionPayload, auditEvent.TimeUTC);
            }

            return retConnection;
        }

        private ConnectionsPayload CreateInetConnPayloadFromAuditEvent(AuditEvent auditEvent)
        {
            ConnectionsPayload connectionPayload = null;
            string hexStringSaddr = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.SocketAddress);
            try
            {
                ConnectionSaddr saddr = ConnectionSaddr.ParseSaddrToInetConnection(hexStringSaddr);
                if (!ConnectionSaddr.IsLocalIp(saddr.Ip)) //we don't send local connections
                {
                    connectionPayload = CreateConnPayloadFromAuditEvent(auditEvent);
                    connectionPayload.RemoteAddress = saddr.Ip;
                    connectionPayload.RemotePort = saddr.Port.ToString();
                }
            }
            catch (Exception e)
            {
                SimpleLogger.Error($"Failed to parse saddr {hexStringSaddr}", exception: e);
                connectionPayload = null;
            }

            return connectionPayload;
        }

        //For other famlies (non INET) that are required more investigation - send event with raw data (hex string)
        private ConnectionsPayload CreateNonInetConnPayloadFromAuditEvent(LinuxAddressFamily family, AuditEvent auditEvent)
        {
            ConnectionsPayload connectionPayload = CreateConnPayloadFromAuditEvent(auditEvent);
            string hexStringSaddr = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.SocketAddress);
            connectionPayload.ExtraDetails = new Dictionary<string, string>
            {
                { "familyName", family.ToString() },
                { "saddr", hexStringSaddr }
            };
            return connectionPayload;
        }

        private ConnectionsPayload CreateConnPayloadFromAuditEvent(AuditEvent auditEvent)
        {
            ConnectionsPayload payload = new ConnectionsPayload();
            payload.Direction = GetConnectionDirection();
            payload.Protocol = EProtocol.Tcp.ToString();
            payload.Executable = EncodedAuditFieldsUtils.DecodeHexStringIfNeeded(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.Executable), Encoding.UTF8);
            payload.CommandLine = EncodedAuditFieldsUtils.DecodeHexStringIfNeeded(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ProcessTitle), Encoding.UTF8);
            payload.ProcessId = UInt32.Parse(auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.ProcessId));
            payload.UserId = auditEvent.GetPropertyValue(AuditEvent.AuditMessageProperty.UserId);

            return payload;
        }
    }
}