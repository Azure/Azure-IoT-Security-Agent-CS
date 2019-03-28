// <copyright file="FirewallConfigurationSnapshotGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Iptables;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Snapshot generator for linux firewall (IPTable) configuration (firewall rules)
    /// The generator works only if no more than the default chains (INPUT, OUTPUT, FORWARD) are defined
    /// </summary>
    public class FirewallConfigurationSnapshotGenerator : SnapshotEventGenerator
    {
        private const string IpTablesExistCommand = "which iptables-save";
        private const string IpTablesSaveCommand = "sudo iptables-save ";

        private const string FilterTableName = "filter";

        private const string InputChain = "INPUT";
        private const string OutputChain = "OUTPUT";

        private readonly bool _isIptablesExist;
        private readonly IProcessUtil _processUtil;

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public FirewallConfigurationSnapshotGenerator() : this(ProcessUtil.Instance) { }

        /// <inheritdoc />
        public FirewallConfigurationSnapshotGenerator(IProcessUtil processUtil)
        {
            _processUtil = processUtil;
            string content = _processUtil.ExecuteBashShellCommand(IpTablesExistCommand);
            _isIptablesExist = !(string.IsNullOrEmpty(content));
        }

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<FirewallConfiguration>();

        /// <summary>
        /// Create linux firewall configuration snapshot
        /// </summary>
        /// <returns>List of firewall configuration snapshot event, the list should contain only one element</returns>
        protected override List<IEvent> GetEventsImpl()
        {
            var returnedEvents = new List<IEvent>();
            if (!_isIptablesExist)
            {
                SimpleLogger.Warning($"{GetType().Name}: Iptables does not exist on this device");
                returnedEvents.Add(new FirewallConfiguration(Priority));
                return returnedEvents;
            }

            string iptablesSaveOutput = _processUtil.ExecuteBashShellCommand(IpTablesSaveCommand);
            if (string.IsNullOrEmpty(iptablesSaveOutput))
            {
                SimpleLogger.Warning(
                    $"{GetType().Name}: Can't get Iptables data, check permission or iptables is not configured on this machine");
                returnedEvents.Add(new FirewallConfiguration(Priority));
                return returnedEvents;
            }

            string[] filterTable = GetIptablesTableSection(iptablesSaveOutput, FilterTableName);

            var snapshot = IptablesChain.GetChainsFromTable(filterTable ?? new string[] {})
                .SelectMany(ParseChainFromTable)
                .ToArray();

            returnedEvents.Add(new FirewallConfiguration(Priority, snapshot));
            return returnedEvents;
        }

        private static string[] GetIptablesTableSection(string content, string tableName)
        {
            var tables = content.Split("*");

            return tables.FirstOrDefault(table => table.StartsWith(tableName))?.SplitStringOnNewLine();
        }

        private static List<FirewallRulePayload> ParseChainFromTable(IptablesChain chain)
        {
            FirewallRulePayload.Directions? direction = null;
            if (chain.Name == InputChain)
            {
                direction = FirewallRulePayload.Directions.In;
            } else if (chain.Name== OutputChain)
            {
                direction = FirewallRulePayload.Directions.Out;
            }

            return chain.Rules.Select(rule => new FirewallRulePayload
            {
                Priority = rule.Priority,
                ChainName = chain.Name,
                Action = rule.TargetAction,
                Direction = direction,
                Enabled = true,
                ExtraDetails = rule.Extras,
                SourceAddress = GetConcatenatedValues(rule.RuleMatchConditions, 
                    IptableRule.MatchConditions.SourceAddress, IptableRule.MatchConditions.SourceAddressRange),
                SourcePort = GetConcatenatedValues(rule.RuleMatchConditions, 
                    IptableRule.MatchConditions.SourcePort, IptableRule.MatchConditions.SourcePortRange),
                Protocol = GetValueOrEmptyString(rule.RuleMatchConditions, IptableRule.MatchConditions.Protocol),
                DestinationAddress= GetConcatenatedValues(rule.RuleMatchConditions, 
                    IptableRule.MatchConditions.DestinationAddress, IptableRule.MatchConditions.DestinationAddressRange),
                DestinationPort = GetConcatenatedValues(rule.RuleMatchConditions, 
                    IptableRule.MatchConditions.DestinationPort, IptableRule.MatchConditions.DestinationPortRange)
            }).ToList();
        }

        private static string GetValueOrEmptyString(Dictionary<IptableRule.MatchConditions, string> ruleRuleMatchConditions,
            IptableRule.MatchConditions matchConditions)
        {
            ruleRuleMatchConditions.TryGetValue(matchConditions, out string val);

            return val ?? string.Empty;
        }

        private static string GetConcatenatedValues(Dictionary<IptableRule.MatchConditions, string> ruleRuleMatchConditions,
            IptableRule.MatchConditions matchConditions, IptableRule.MatchConditions matchConditions2)
        {
            var values = new List<string>();
            ruleRuleMatchConditions.TryGetValue(matchConditions, out string val);
            values.Add(val);
            ruleRuleMatchConditions.TryGetValue(matchConditions2, out val);
            values.Add(val);

            return string.Join(",", values.Where(str => str != null));
        }
    }
}