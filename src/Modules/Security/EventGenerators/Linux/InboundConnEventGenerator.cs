// <copyright file="InboundConnEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Utils;
using System.Collections.Generic;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.ConnectionsPayload;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux
{
    /// <summary>
    /// Inbound tcp connections event generator
    /// </summary>
    public class InboundConnEventGenerator : ConnectionsEventGeneratorBase
    {
        /// Auditd accept rule
        public static readonly string AuditRuleAccept = "-A exit,always -F arch=b{0} -S accept -F success=1";

        /// <inheritdoc />
        /// Override the default ausearch "-m EVENTTYPE" convention with "-sc accept"
        protected override string RecordFilteringArguments => "-sc accept";

        /// <summary>
        /// <inheritdoc />
        /// Network audit log should be configured for the syscall accept()
        /// We collect only SUCCESSFUL connection
        /// </summary>
        protected override IEnumerable<string> AuditRulesPrerequisites { get; } = AuditEventGeneratorBase.GeneratePrerequisitesRules(InboundConnEventGenerator.AuditRuleAccept);


        /// <summary>
        /// Ctor - creates a new event generator
        /// use default ProcessUtil
        /// </summary>
        public InboundConnEventGenerator() : this(ProcessUtil.Instance) { }

        /// <inheritdoc />
        public InboundConnEventGenerator(IProcessUtil processUtil) : base(processUtil)
        {
        }

        /// <inheritdoc />
        protected override ConnectionDirection GetConnectionDirection() => ConnectionDirection.In;
    }
}