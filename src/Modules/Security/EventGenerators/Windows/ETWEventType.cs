// <copyright file="ETWEventType.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// The type of ETW events that we support, the int value represents the event ID
    /// </summary>
    public enum ETWEventType
    {
#pragma warning disable CS1591
        LoginFailure = 4625,
        LoginSuccess = 4624,
        ProcessCreate = 4688,
        ProcessExit = 4689,
        ConnectionAllowed = 5156,
        ConnectionBlocked = 5157
#pragma warning restore CS1591
    }
}
