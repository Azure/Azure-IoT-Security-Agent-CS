// <copyright file="FirewallConfigurationEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Snapshot generator for Windows firewall configuration (firewall rules)
    /// </summary>
    public class FirewallConfigurationEventGenerator : SnapshotEventGenerator
    {
        private const string RuleNameExtraDetailsKey = "Rule Name";

        private readonly INetFwPolicy2 _firewallPolicy;
        private readonly INetFwMgr _firewallManager;

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<FirewallConfiguration>();

        /// <summary>
        /// Ctor - creates a new event generator
        /// </summary>
        public FirewallConfigurationEventGenerator() : this((INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")), 
                                                            (INetFwMgr)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwMgr")))
        {
        }

        /// <inheritdoc />
        public FirewallConfigurationEventGenerator(INetFwPolicy2 firewallPolicy, INetFwMgr firewallManager)
        {
            _firewallManager = firewallManager;
            _firewallPolicy = firewallPolicy;
        }

        /// <inheritdoc />
        protected override List<IEvent> GetEventsImpl()
        {
            if (!_firewallManager.LocalPolicy.CurrentProfile.FirewallEnabled)
            {
                SimpleLogger.Warning($"{GetType().Name}: the firewall is turned off");
                return new List<IEvent>() { new FirewallConfiguration(Priority) };
            }

            FirewallConfiguration firewallConfiguration = new FirewallConfiguration(Priority, GetPayloads().ToArray());
            return new List<IEvent>() { firewallConfiguration };
        }

        private IEnumerable<FirewallRulePayload> GetPayloads()
        {
            INetFwRules rules = _firewallPolicy.Rules;

            int count = rules.Count;
            IEnumVARIANT enumVARIANT = rules.get__NewEnum();
            object[] obj = new object[1];

            for (int i = 0; i < count; i++)
            {
                if (enumVARIANT.Next(1, obj, IntPtr.Zero) == 0)
                {
                    var rule = (INetFwRule)obj[0];

                    if (!ShouldHandleRule(rule))
                    {
                        continue;
                    }

                    FirewallRulePayload payload = new FirewallRulePayload();
                    payload.Action = rule.Action.ToIoTValue();
                    payload.Direction = rule.Direction.ToIoTValue();
                    payload.Enabled = rule.Enabled;
                    payload.Application = rule.ApplicationName;
                    payload.SourceAddress = rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN ? rule.RemoteAddresses : rule.LocalAddresses;
                    payload.SourcePort = rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN ? rule.RemotePorts : rule.LocalPorts;
                    payload.DestinationAddress = rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN ? rule.LocalAddresses : rule.RemoteAddresses;
                    payload.DestinationPort = rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN ? rule.LocalPorts : rule.RemotePorts;
                    payload.Protocol = ((FirewallRuleProtocol)rule.Protocol).ToString();
                    payload.Priority = null;
                    payload.ExtraDetails = new Dictionary<string, string>() { { RuleNameExtraDetailsKey, rule.Name } };

                    yield return payload;
                }
            }
        }

        private bool ShouldHandleRule(INetFwRule rule)
        {
            return rule != null &&
                    (rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN || rule.Direction == NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT) &&
                    (rule.Protocol == (int)FirewallRuleProtocol.Tcp || rule.Protocol == (int)FirewallRuleProtocol.Udp || rule.Protocol == (int)FirewallRuleProtocol.Any);
        }
    }
}
