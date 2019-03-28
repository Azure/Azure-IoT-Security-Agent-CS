// <copyright file="AuditEvent.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit
{
    //TODO: replace this with a record type when C# 8 rolls around.
    /// <summary>
    /// Represents a linux audit event
    /// </summary>
    public class AuditEvent
    {
        /// <summary>
        /// The properties that we currently know how to parse from the ausearch output
        /// </summary>
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public enum AuditMessageProperty
        {
            [Display(Name = "pid")]
            ProcessId,
            [Display(Name = "uid")]
            UserId,
            [Display(Name = "auid")]
            AuditUserId,
            [Display(Name = "id")]
            LoginUserId,
            [Display(Name = "ses")]
            SessionId,
            [Display(Name = "op")]
            Operation,
            [Display(Name = "acct")]
            Account,
            [Display(Name = "exe")]
            Executable,
            [Display(Name = "hostname")]
            Hostname,
            [Display(Name = "addr")]
            Address,
            [Display(Name = "terminal")]
            Terminal,
            [Display(Name = "res")]
            Result,
            [Display(Name = "proctitle")]
            ProcessTitle,
            CommandLine,
            [Display(Name = "ppid")]
            ParentProcessId,
            [Display(Name = "saddr")]
            SocketAddress,
            [Display(Name = "argc")]
            ProcessArgCount
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private const string PropertyRegexTemplate = @"(?<=[\s']{0}=)[^\s']*";
        private const string QuotedPropertyRegexTemplate = "(?<=\\s{0}=\\\")[^\\\"]*";
        private static readonly string[] propertyRegexTemplates = { QuotedPropertyRegexTemplate , PropertyRegexTemplate };
        private static readonly Regex IdRegex = new Regex(@"(?<=msg=audit\().*(?=\))");
        private static readonly Regex TypeRegex = new Regex(string.Format(PropertyRegexTemplate, "type"));

        /// <summary>
        /// The type of the audit event
        /// </summary>
        public AuditEventType Type { get; }

        /// <summary>
        /// The UTC time in which the audit event was generated
        /// </summary>
        public DateTime TimeUTC { get; }

        /// <summary>
        /// The Id of the audit event, of the format [time_since_epoch]:[unique_id] example: 1535612146.807:2224
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The data from the audit event
        /// </summary>
        private Dictionary<AuditMessageProperty, string> MessageData { get; }

        /// <summary>
        /// The text from ausearch containing the event
        /// </summary>
        private string EventText { get; }

        /// <summary>
        /// Creates an instance of an AuditEvent
        /// </summary>
        /// <param name="type">The type of the audit event</param>
        /// <param name="time">The time in whic the audit event was generated</param>
        /// <param name="id">The ID of the audit event</param>
        /// <param name="messageData">The data the audit event contains</param>
        /// <param name="eventText">The ausearch line containing the event</param>
        private AuditEvent(AuditEventType type, DateTime time, string id,
            Dictionary<AuditMessageProperty, string> messageData, string eventText)
        {
            Type = type;
            TimeUTC = time;
            Id = id;
            MessageData = messageData;
            EventText = eventText;
        }

        /// <summary>
        /// Gets the value of the property if it exists
        /// if the property does not exist and throwIfNotExist = true => throw an exception
        /// if the property does not exist and throwIfNotExist = false => return null
        /// </summary>
        /// <exception >
        /// if the key not found and throwIfNotExist = true, throws an exception with event dump
        /// </exception>
        /// <param name="property">The property value to get</param>
        /// <param name="throwIfNotExist">if true throw an exception if the property does not exist</param>
        /// <returns>The value, or null if it does not exist and throwIfNotExist = false</returns>
        public string GetPropertyValue(AuditMessageProperty property, bool throwIfNotExist = true)
        {
            bool propertyExist = MessageData.TryGetValue(property, out string prop);

            if (!propertyExist && throwIfNotExist)
            {
                throw new AuditEventPropertyNotFoundException(Type, Id, MessageData, property, EventText);
            }

            return prop;
        }

        /// <summary>
        /// Tries to get a value of a specific property from a ausearch output line
        /// </summary>
        /// <param name="eventText">The ausearch line</param>
        /// <param name="property">The property to get</param>
        /// <param name="value">The value of the property, null if not found</param>
        /// <returns>True if property found, false if not</returns>
        private static bool TryGetPropertyValue(string eventText, AuditMessageProperty property, out string value)
        {
            DisplayAttribute displayNameAttribute = property.GetAttribute<DisplayAttribute>();

            if (displayNameAttribute == null)
            {
                value = null;
                return false;
            }

            foreach (var regexTemplate in propertyRegexTemplates)
            {
                var regexMatch = Regex.Match(eventText, string.Format(regexTemplate, displayNameAttribute.Name));
                if (regexMatch.Success)
                {
                    value = regexMatch.Value;
                    return regexMatch.Success;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Parse a line from ausearch output into an AuditEvent
        /// </summary>
        /// <param name="eventText">Line from ausearch</param>
        /// <returns>An AuditEvent</returns>
        public static AuditEvent ParseFromAusearchLine(string eventText)
        {
            var messageData = new Dictionary<AuditMessageProperty, string>();
            foreach (AuditMessageProperty prop in Enum.GetValues(typeof(AuditMessageProperty)))
            {
                if (TryGetPropertyValue(eventText, prop, out string match))
                {
                    string expectedValue;
                    if (prop == AuditMessageProperty.SocketAddress)
                        expectedValue = match; // In case of 'saddr', hex-encoded bytes are expected
                    else
                        expectedValue = EncodedAuditFieldsUtils.DecodeHexStringIfNeeded(match, Encoding.UTF8);

                    messageData[prop] = expectedValue;

                    if (prop == AuditMessageProperty.ProcessArgCount && UInt32.TryParse(match, out uint argCount))
                    {
                        string[] processCommandArgs = new string[argCount];
                        for (int i = 0; i < argCount; i++)
                        {
                            string parameter = Regex.Match(eventText, string.Format(QuotedPropertyRegexTemplate, $"a{i}")).Value;
                            if (string.IsNullOrEmpty(parameter))
                            {
                                var encodedString = Regex.Match(eventText, string.Format(PropertyRegexTemplate, $"a{i}")).Value;
                                parameter = EncodedAuditFieldsUtils.DecodeHexStringIfNeeded(encodedString, Encoding.UTF8);
                            }
                                
                            processCommandArgs[i] = parameter;
                        }
                        messageData[AuditMessageProperty.CommandLine] = String.Join(' ', processCommandArgs);
                    }
                }
            }

            AuditEventType type = (AuditEventType) TypeRegex.Match(eventText).Value.ParseFromDisplayName<AuditEventType>();
            string eventId = IdRegex.Match(eventText).Value;
            DateTime eventTimeUTC = TimeUtils.FromUnixTime(eventId.Split(':').First());

            var ev = new AuditEvent
            (
                type,
                eventTimeUTC,
                eventId,
                messageData,
                eventText
            );

            return ev;
        }
    }
}