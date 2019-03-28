// <copyright file="WmiUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows.Utils
{
    /// <inheritdoc />
    public class WmiUtils : IWmiUtils
    {
        private const string PropSearchRegex = @"\n{0}\s+:\s?(.+?(?=\n\w))";

        private static readonly Lazy<WmiUtils> _instance = new Lazy<WmiUtils>(new WmiUtils(ProcessUtil.Instance));

        private readonly IProcessUtil _processUtils;

        /// <summary>
        /// Get singleton instance
        /// </summary>
        public static WmiUtils Instance => _instance.Value;

        private WmiUtils(IProcessUtil processUtils)
        {
            _processUtils = processUtils;
        }

        private static readonly Dictionary<WmiDataType, Tuple<string, string>> Queries =
            new Dictionary<WmiDataType, Tuple<string, string>>
            {
                { WmiDataType.OsName, Tuple.Create("SELECT Caption FROM Win32_OperatingSystem", "Caption") },
                { WmiDataType.OsVersion, Tuple.Create("SELECT Version FROM Win32_OperatingSystem", "Version") },
                { WmiDataType.OsVendor, Tuple.Create("SELECT Manufacturer FROM Win32_OperatingSystem", "Manufacturer") },
                { WmiDataType.TotalPhysicalMemory, Tuple.Create("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem", "TotalVisibleMemorySize") },
                { WmiDataType.FreePhysicalMemory, Tuple.Create("SELECT FreePhysicalMemory FROM Win32_OperatingSystem", "FreePhysicalMemory") },
                { WmiDataType.TotalVirtualMemory, Tuple.Create("SELECT TotalVirtualMemorySize FROM Win32_OperatingSystem", "TotalVirtualMemorySize") },
                { WmiDataType.FreeVirtualMemory, Tuple.Create("SELECT FreeVirtualMemory FROM Win32_OperatingSystem", "FreeVirtualMemory") },
                { WmiDataType.BootDevice, Tuple.Create("SELECT BootDevice FROM Win32_OperatingSystem", "BootDevice") },
                { WmiDataType.BiosVersion, Tuple.Create("SELECT Version FROM Win32_BIOS", "Version") },
                { WmiDataType.Cpu, Tuple.Create("SELECT Name FROM Win32_Processor", "Name") },
                { WmiDataType.Gpu, Tuple.Create("SELECT Name FROM Win32_VideoController", "Name") },
                { WmiDataType.ConnectedDevices, Tuple.Create("SELECT Name FROM Win32_PnPEntity", "Name") },
                { WmiDataType.UsbHubs, Tuple.Create("SELECT Name FROM Win32_USBHub", "Name") },
                { WmiDataType.EnvirnmentVariables, Tuple.Create("SELECT Name FROM Win32_Environment", "Name") },
                { WmiDataType.SystemDrivers, Tuple.Create("SELECT Name FROM Win32_SystemDriver", "Name") },
                { WmiDataType.LocalUsers, Tuple.Create("SELECT Name FROM Win32_UserAccount Where LocalAccount = True", "Name") },
                { WmiDataType.InstalledHotfixes, Tuple.Create("SELECT HotFixID FROM Win32_QuickFixEngineering", "HotFixID") }
            };

        /// <inheritdoc />
        public string GetSingleValue(WmiDataType type)
        {
            return GetEnumerableValue(type).First();
        }

        /// <inheritdoc />
        public IEnumerable<string> GetEnumerableValue(WmiDataType type)
        {
            return RunWmiQuery(Queries[type].Item1, Queries[type].Item2).Select(item => item[Queries[type].Item2]);
        }

        /// <summary>
        /// Runs a WMI query
        /// </summary>
        /// <param name="query">The query to run</param>
        /// <param name="values">The values to extract from the result</param>
        /// <returns>A collection of items, each containing the values requested in 'values'</returns>
        public IEnumerable<Dictionary<string, string>> RunWmiQuery(string query, params string[] values)
        {
            var command = $"Get-CimInstance -Query \\\"{query}\\\" | Format-List -Property *";

            string result = _processUtils.ExecutePowershellCommand(command);

            string[] splitResult = result.Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries);

            var returnValue = new List<Dictionary<string, string>>();

            foreach (string item in splitResult)
            {
                var dict = new Dictionary<string, string>();

                foreach (var value in values)
                {
                    dict[value] = Regex.Match(item, string.Format(PropSearchRegex, value), RegexOptions.Singleline).Groups[1].Value.Trim();
                }

                returnValue.Add(dict);
            }

            return returnValue;
        }
    }
}