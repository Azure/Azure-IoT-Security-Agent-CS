// <copyright file="SystemEventUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils
{
    /// <summary>
    /// A collection of utilities to aid the prasing of system events
    /// </summary>
    public static class SystemEventUtils
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string TimeGeneratedFieldName = "TimeGenerated";
        public const string MessageFieldName = "Message";
        public const string ProcessIdFieldName = "Process ID";
        public const string CreatorProcessIdFieldName = "Creator Process ID";
        public const string ProcessNameFieldName = "Process Name";
        public const string SecurityIdFieldName = "Security ID";
        public const string NewProcessCommandLineFieldName = "Process Command Line";
        public const string ExitStatusFieldName = "Exit Status";
        public const string AccountDomainFieldName = "Account Domain";
        public const string AccountNameFieldName = "Account Name";
        public const string LogonIdFieldName = "Logon ID";
        public const string TokenElevationTypeFieldName = "Token Elevation Type";
        public const string MandatoryLabelFieldName = "Mandatory Label";
        public const string CallerProcessIdFieldName = "Caller Process ID";
        public const string CallerProcessNameFieldName = "Caller Process Name";
        public const string SourceNetworkAddressFieldName = "Source Network Address";
        public const string SourcePortFieldName = "Source Port";
        public const string LogonProcessFieldName = "Logon Process";
        public const string FailureReasonFieldName = "Failure Reason";
        public const string ApplicationNameFieldName = "Application Name";
        public const string DirectionFieldName = "Direction";
        public const string SourceAddressFieldName = "Source Address";
        public const string DestinationAddressFieldName = "Destination Address";
        public const string DestinationPortFieldName = "Destination Port";
        public const string ProtocolFieldName = "Protocol";
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// Get the value of a field from the message of a system event
        /// </summary>
        /// <param name="eventMessage">The message</param>
        /// <param name="property">The desired property</param>
        /// <param name="matchIndex">If there are several instances of the property, the index of the one that should be returned</param>
        /// <returns></returns>
        public static string GetEventPropertyFromMessage(string eventMessage, string property, int matchIndex = 0)
        {
            var matches = Regex.Matches(eventMessage, $"{property}:\\t+(.*)(\\r\\n|$)");
            return matches?[matchIndex].Groups[1].Value;
        }
    }
}
