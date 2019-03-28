// <copyright file="PayloadNetworkEnum.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// Represents the event priority
    /// </summary>
    public enum EProtocol
    {
#pragma warning disable CS1591
        Tcp,
        Udp
#pragma warning restore CS1591
    }
}