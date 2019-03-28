// <copyright file="IptableRule.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Iptables
{
    class IptableRule
    {
        /// <summary>
        /// Iptable rule mathing conditions arguments that we know to parse
        /// </summary>
        public enum MatchConditions
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            [Display(Name = "p")]
            Protocol,
            [Display(Name = "s")]
            SourceAddress,
            [Display(Name = "src-range")]
            SourceAddressRange,
            [Display(Name = "sport")]
            SourcePort,
            [Display(Name = "sports")]
            SourcePortRange,
            [Display(Name = "d")]
            DestinationAddress,
            [Display(Name = "dst-range")]
            DestinationAddressRange,
            [Display(Name = "dports")]
            DestinationPortRange,
            [Display(Name = "dport")]
            DestinationPort,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// Rule priority
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// The rule packet matching arguments
        /// </summary>
        public Dictionary<MatchConditions, string> RuleMatchConditions { get; }

        /// <summary>
        /// Extra details, 
        /// such as other action and match conditions that we don't know to parse
        /// </summary>
        public Dictionary<string, string> Extras { get; }

        /// <summary>
        /// Rule action 
        /// </summary>
        public FirewallRulePayload.Actions TargetAction { get; }

        /// <summary>
        /// Dictionary of MatchCondition display name to their MatchConditions enums
        /// </summary>
        private static readonly Dictionary<string, MatchConditions> KnownConditions
            = ((MatchConditions[]) Enum.GetValues(typeof(MatchConditions)))
            .ToDictionary(e => e.GetAttribute<DisplayAttribute>().Name, e => e);

        /// <summary>
        /// Dictionary of rule actions (as they are displayed on iptables-save) to their Action enum
        /// </summary>
        private static readonly Dictionary<string, FirewallRulePayload.Actions> KnownActions
            = new Dictionary<string, FirewallRulePayload.Actions>
            {
                { "ACCEPT", FirewallRulePayload.Actions.Allow },
                { "REJECT", FirewallRulePayload.Actions.Deny },
                { "DROP", FirewallRulePayload.Actions.Deny }
            };

        /// <summary>
        /// Key value argument regex 
        /// </summary>
        private static readonly Regex KeyValueArgumentRegex = new Regex(@"(!\s)?-?-([^\s]*)\s([^\s\""][^\s]*|\""[^\""]*\"")");

        private const string Append = "A";
        private const string Jump = "j";
        private const string GoTo = "g";

        /// <summary>
        /// Creates a new Iptable rule object
        /// </summary>
        /// <param name="priority">Rule priority</param>
        /// <param name="targetAction">Rule target (accept, reject, etc..)</param>
        /// <param name="ruleMatchConditions">Dictionary of matching keys to their values</param>
        /// <param name="extras">Extra details</param>
        public IptableRule(int priority, FirewallRulePayload.Actions targetAction,
            Dictionary<MatchConditions, string> ruleMatchConditions, Dictionary<string, string> extras)
        {
            Priority = priority;
            TargetAction = targetAction;
            RuleMatchConditions = ruleMatchConditions ?? new Dictionary<MatchConditions, string>();
            Extras = extras ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Parse a rule line from iptables-save 
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="priority">This rule priority</param>
        /// <returns>IptabeRule</returns>
        public static IptableRule ParseRuleFromIptablesSaveLine(string line, int priority)
        {
            var arguments = ParseRuleArguments(line);

            var matchConditions = new Dictionary<MatchConditions, string>();
            var extraConditions = new Dictionary<string, string>();
            var action = FirewallRulePayload.Actions.Other;
            foreach (var argumentKey in arguments.Keys)
            {
                if (argumentKey == Jump|| argumentKey == GoTo)
                {
                    action = ParseAction(arguments[argumentKey]);
                    if (action == FirewallRulePayload.Actions.Other)
                    {
                        extraConditions["Action"] = arguments[argumentKey];
                    }
                }
                else if (KnownConditions.ContainsKey(argumentKey)) //arguments we know how to parse
                {
                    matchConditions[KnownConditions[argumentKey]] = arguments[argumentKey];
                }
                else
                {
                    extraConditions[argumentKey] = arguments[argumentKey];
                }
            }

            return new IptableRule(priority, action, matchConditions, extraConditions);
        }

        /// <summary>
        /// Parse an action, if the action is known return it's corresponding enum. 
        /// else return Actions.Othe
        /// </summary>
        /// <param name="action">The action to parse</param>
        /// <returns>FirewallRulePayload.Actions</returns>
        public static FirewallRulePayload.Actions ParseAction(string action)
        {
            return KnownActions.ContainsKey(action) ? KnownActions[action] : FirewallRulePayload.Actions.Other;
        }

        private static Dictionary<string, string> ParseRuleArguments(string line)
        {
            return KeyValueArgumentRegex
                .Matches(line)
                .Select(x => string.IsNullOrEmpty(x.Groups[1].Value)
                    ? new KeyValuePair<string, string>(x.Groups[2].Value, x.Groups[3].Value) 
                    : new KeyValuePair<string, string>(x.Groups[2].Value, $"!({x.Groups[3].Value})"))
                .Where(x => x.Key != Append) //An argument indicating parent chain name
                .GroupBy(x => x.Key)
                .ToDictionary(group => group.Key, group => string.Join(",", group.Select(val => val.Value)));
        }
    }
}