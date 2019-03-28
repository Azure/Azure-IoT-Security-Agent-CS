// <copyright file="EventCategory.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Contracts.Events
{
    /// <summary>
    /// Represents the event category
    /// </summary>
    public enum EventCategory
    {
#pragma warning disable CS1591
        Triggered,
        Periodic
#pragma warning restore CS1591 
    }
}