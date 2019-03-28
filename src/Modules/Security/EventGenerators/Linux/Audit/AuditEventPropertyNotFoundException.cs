// <copyright file="AuditEventPropertyNotFoundException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit
{
    /// <summary>
    /// Represents an exceptional state when trying to fetch an audit event property that does not exist
    /// </summary>
    public class AuditEventPropertyNotFoundException : Exception
    {
        /// <summary>
        /// The content of the event
        /// </summary>
        private readonly IDictionary<AuditEvent.AuditMessageProperty, string> _eventData;

        /// <summary>
        /// The id of the event
        /// </summary>
        private readonly string _eventId;

        /// <summary>
        /// The type of the event
        /// </summary>
        private readonly AuditEventType _eventType;

        /// <summary>
        /// The property that caused the exception
        /// </summary>
        private readonly AuditEvent.AuditMessageProperty _missingProperty;

        /// <summary>
        /// The ausearch line containing the event
        /// </summary>
        private readonly string _eventText;

        /// <summary>
        /// Creates a new AuditEventPropertyNotFoundException exception
        /// </summary>
        /// <param name="eventType">The type of the audit event</param>
        /// <param name="eventId">The id of the event</param>
        /// <param name="eventData">The content of the event</param>
        /// <param name="missingProperty">The property that caused the exception</param>
        /// <param name="eventText">The ausearch line containing the event</param>
        public AuditEventPropertyNotFoundException(AuditEventType eventType, string eventId,
            IDictionary<AuditEvent.AuditMessageProperty, string> eventData,
            AuditEvent.AuditMessageProperty missingProperty, string eventText)
        {
            _eventType = eventType;
            _eventId = eventId;
            _eventData = eventData;
            _missingProperty = missingProperty;
            _eventText = eventText;
        }

        /// <inheritdoc />
        public override string Message => GetExceptionMessage();

        /// <inheritdoc />
        public override string ToString()
        {
            string exception = $"{Message}{Environment.NewLine}";
            exception += $"event dump:{Environment.NewLine}";

            foreach (var property in _eventData.Keys)
            {
                exception += $"Property: {property.ToString()}, Value: {_eventData[property]} {Environment.NewLine}";
            }

            exception += $"[Audit event: {_eventText}{Environment.NewLine}]";

            return $"{exception}{Environment.NewLine}{base.ToString()}";
        }

        private string GetExceptionMessage()
        {
            return $"Couldn't find the property {_missingProperty.ToString()} in  {_eventType.ToString()} AuditEvent, event id: {_eventId}";
        }
    }
}