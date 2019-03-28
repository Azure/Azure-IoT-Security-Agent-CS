// <copyright file="EventPriority.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Represents the event priority
    /// </summary>
    public enum EventPriority
    {
#pragma warning disable CS1591
        Off = 0,
        Low = 1,
        High = 2,
        Operational = 3
#pragma warning restore CS1591
    }
}