// <copyright file="FirewallConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Events
{
    /// <inheritdoc />
    public class FirewallConfiguration : SecurityPeriodicEvent<FirewallRulePayload>
    {
        /// <inheritdoc />
        public FirewallConfiguration(EventPriority priority, params FirewallRulePayload[] payloads)
            : base(priority, payloads)
        {
        }
    }
}