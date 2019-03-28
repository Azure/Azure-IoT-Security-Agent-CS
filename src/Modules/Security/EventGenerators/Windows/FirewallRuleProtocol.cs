// <copyright file="FirewallRuleProtocol.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Defines the types of protocols we support for windows firewall rules
    /// </summary>
    public enum FirewallRuleProtocol
    {
        /// <summary>
        /// TCP rule
        /// </summary>
        Tcp = 0x06,
        /// <summary>
        /// UDP rule
        /// </summary>
        Udp = 0x11,
        /// <summary>
        /// Any - includes Tcp, UDP and others.
        /// </summary>
        Any = 0x100
    }
}
