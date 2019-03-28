// <copyright file="WmiDataType.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// The data types we support fetching using WmiUtils
    /// </summary>
    public enum WmiDataType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        OsName,
        OsVersion,
        OsVendor,
        TotalPhysicalMemory,
        FreePhysicalMemory,
        TotalVirtualMemory,
        FreeVirtualMemory,
        BootDevice,
        BiosVersion,
        Cpu,
        Gpu,
        ConnectedDevices,
        UsbHubs,
        EnvirnmentVariables,
        SystemDrivers,
        LocalUsers,
        InstalledHotfixes
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
