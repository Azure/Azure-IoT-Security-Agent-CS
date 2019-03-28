// <copyright file="FirewallRulePropertyExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using NetFwTypeLib;
using System;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils
{
    /// <summary>
    /// Extension methods to aid in mapping firewalls rules to thir respective payloads
    /// </summary>
    public static class FirewallRulePropertyExtensions
    {
        /// <summary>
        /// Convert a firewall action value to the correct payload value
        /// </summary>
        /// <param name="ruleAction">the rule action</param>
        /// <returns>the payload value corresponding to the action</returns>
        public static FirewallRulePayload.Actions ToIoTValue(this NET_FW_ACTION_ ruleAction)
        {
            switch (ruleAction)
            {
                case NET_FW_ACTION_.NET_FW_ACTION_ALLOW:
                    return FirewallRulePayload.Actions.Allow;
                case NET_FW_ACTION_.NET_FW_ACTION_BLOCK:
                    return FirewallRulePayload.Actions.Deny;
                default:
                    return FirewallRulePayload.Actions.Other;
            }
        }

        /// <summary>
        /// Convert a firewall direction value to the correct payload value
        /// </summary>
        /// <param name="ruleDirection">the rule direction</param>
        /// <returns>the payload value corresponding to the direction</returns>
        public static FirewallRulePayload.Directions ToIoTValue(this NET_FW_RULE_DIRECTION_ ruleDirection)
        {
            switch (ruleDirection)
            {
                case NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN:
                    return FirewallRulePayload.Directions.In;
                case NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT:
                    return FirewallRulePayload.Directions.Out;
                default:
                    throw new ApplicationException($"rule direction of {ruleDirection} cannot be converted");
            }
        }
    }
}
