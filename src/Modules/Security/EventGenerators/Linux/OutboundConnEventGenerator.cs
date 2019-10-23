// <copyright file="OutboundConnEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Utils;
using System.Collections.Generic;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.ConnectionsPayload;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Collect all the connections that were open from the machine to the internet
    /// The collection is done by reading the auditd logs for all "connect" type with "success"
    /// </summary>
    public class OutboundConnEventGenerator : ConnectionsEventGeneratorBase
    {
        /// Auditd connect rule
        public static readonly string AuditRuleConnect = "-A exit,always -F arch=b{0} -S connect -F success=1";

        /// <inheritdoc />
        /// Override the default ausearch "-m EVENTTYPE" convention with "-sc connect"
        protected override string RecordFilteringArguments => "-sc connect";

        /// <summary>
        /// <inheritdoc />
        /// Network audit log should be configured for the syscall connect()
        /// Collect only succesful connections
        /// </summary>
        protected override IEnumerable<string> AuditRulesPrerequisites { get; } = AuditEventGeneratorBase.GeneratePrerequisitesRules(OutboundConnEventGenerator.AuditRuleConnect);

        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public OutboundConnEventGenerator() : this(ProcessUtil.Instance) { }

        /// <inheritdoc />
        public OutboundConnEventGenerator(IProcessUtil processUtil) : base(processUtil)
        {
        }

        /// <inheritdoc />
        protected override ConnectionDirection GetConnectionDirection() => ConnectionDirection.Out;
    }
}