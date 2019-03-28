// <copyright file="WindowsSystemInformationEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows
{
    /// <summary>
    /// Generates SystemInformationEvent
    /// </summary>
    public class WindowsSystemInformationEventGenerator : SnapshotEventGenerator
    {
        private readonly IWmiUtils _wmiUtils;

        /// <inheritdoc />
        public override EventPriority Priority => AgentConfiguration.GetEventPriority<SystemInformation>();
      
        /// <summary>
        /// C-tor creates a new instance.
        /// use default WmiUtils
        /// </summary>
        public WindowsSystemInformationEventGenerator() : this(WmiUtils.Instance) { }

        /// <summary>
        /// C-tor creates a new instance.
        /// </summary>
        public WindowsSystemInformationEventGenerator(IWmiUtils wmiUtils)
        {
            _wmiUtils = wmiUtils;
        }
     
        /// <inheritdoc />
        protected override List<IEvent> GetEventsImpl()
        {
            SystemInformationPayload payload = new SystemInformationPayload
                {
                    OSName = _wmiUtils.GetSingleValue(WmiDataType.OsName),
                    OSVersion = _wmiUtils.GetSingleValue(WmiDataType.OsVersion),
                    OSVendor = _wmiUtils.GetSingleValue(WmiDataType.OsVendor),
                    HostName = Environment.MachineName,
                    BIOSVersion = _wmiUtils.GetSingleValue(WmiDataType.BiosVersion),
                    BootDevice = _wmiUtils.GetSingleValue(WmiDataType.BootDevice),
                    TotalPhysicalMemoryInKB = int.Parse(_wmiUtils.GetSingleValue(WmiDataType.TotalPhysicalMemory)),
                    FreePhysicalMemoryInKB = int.Parse(_wmiUtils.GetSingleValue(WmiDataType.FreePhysicalMemory)),
                    TotalVirtualMemoryInKB = int.Parse(_wmiUtils.GetSingleValue(WmiDataType.TotalVirtualMemory)),
                    FreeVirtualMemoryInKB = int.Parse(_wmiUtils.GetSingleValue(WmiDataType.FreeVirtualMemory))
                };

            var ev = new SystemInformation(Priority, payload);
            return new List<IEvent> { ev };
        }
    }
}