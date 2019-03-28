// <copyright file="SystemInformationPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// System information payload
    /// </summary>
    public class SystemInformationPayload : Payload
    {
        /// <summary>
        /// The name of the OS
        /// </summary>
        public string OSName { get; set; }

        /// <summary>
        /// The version of the OS
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// The vendor name of the OS
        /// </summary>
        public string OSVendor { get; set; }

        /// <summary>
        /// The Architecture of the OS
        /// </summary>
        public string OsArchitecture { get; set; }

        /// <summary>
        /// The name host that the agent is running on
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The version of the BIOS
        /// </summary>
        public string BIOSVersion { get; set; }

        /// <summary>
        /// The boot device
        /// </summary>
        public string BootDevice { get; set; }

        /// <summary>
        /// The total amount of physical memory the system has installed in KB
        /// </summary>
        public int TotalPhysicalMemoryInKB { get; set; }

        /// <summary>
        /// The amount of physical memory that is currently free on the system
        /// </summary>
        public int FreePhysicalMemoryInKB { get; set; }

        /// <summary>
        /// The total amount of virtual memory the system can address in KB
        /// </summary>
        public int TotalVirtualMemoryInKB { get; set; }

        /// <summary>
        /// The amount of virtual memory that is currently free on the system
        /// </summary>
        public int FreeVirtualMemoryInKB { get; set; }
    }
}
