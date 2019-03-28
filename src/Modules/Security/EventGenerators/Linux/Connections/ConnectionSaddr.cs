// <copyright file="ConnectionSaddr.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Connections
{
    /// <summary>
    /// This class holds the content of saddr
    /// </summary>
    public class ConnectionSaddr
    {
        /// <summary>
        /// Port number
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        public string Ip { get; set; }


        /// <summary>
        /// The function parse the saddr structure used by Linux connect and accept APIs
        /// It parses only saddr that represent INET or INET6 family type
        /// </summary>
        /// This function receives saddr in hex and translate it 
        /// Note: the translation will be done only if the family name is of type INET for other families null will be returned
        /// <param name="saddrHexString">Saddr hex dump</param>
        /// <returns>ConnectionSaddr</returns>
        public static ConnectionSaddr ParseSaddrToInetConnection(string saddrHexString)
        {
            ConnectionSaddr retSaddr = null;
            LinuxAddressFamily family = ExtractFamilyFromSaddr(saddrHexString);

            // Saddr Template:
            // 0-1 bytes - family,
            // 2-3 bytes - port
            // 4-7 byte - ip                  
            //Extract port
            var bytes = ByteExtensions.GetBytesFromFromHexString(saddrHexString);
            var port = bytes.ToUshort(2, false);

            if (family == LinuxAddressFamily.AF_INET)
            {
                var ip = bytes.Skip(4).Take(4);
                retSaddr = new ConnectionSaddr
                {
                    Ip = string.Join('.', ip),
                    Port = port
                };
            }
            else if (family == LinuxAddressFamily.AF_INET6)
            {
                // Saddr for IPv6:
                // 0-1 bytes - family,
                // 2-3 bytes - port
                // 4-7 byte - flowinfo
                // 8-23 24 - ip
                var ip = BitConverter.ToString(bytes.Skip(8).Take(16).ToArray()).Replace("-", "");
                ip = Regex.Replace(ip, "(.{4})", "$1:");
                ip = ip.Remove(ip.Length - 1, 1);
                retSaddr = new ConnectionSaddr
                {
                    Ip = ip,
                    Port = port
                };
            }

            return retSaddr;
        }

        /// <summary>
        /// Extract the address family from the saddr
        /// </summary>
        /// <param name="saddrHexString">Saddr hex dump</param>
        /// <returns>LinuxAddressFamily type</returns>
        public static LinuxAddressFamily ExtractFamilyFromSaddr(string saddrHexString)
        {
            var bytes = ByteExtensions.GetBytesFromFromHexString(saddrHexString);
            int family = bytes[0];
            return (LinuxAddressFamily) family;
        }

        /// <summary>
        /// Return true if the family indicates internet connection (i.e. INET)
        /// Note: we currently don't support IPV6 so AF_INET6 is ignored
        /// </summary>
        /// <param name="addressFamily">The address family to check</param>
        /// <returns>LinuxAddressFamily type</returns>
        public static bool IsInetFamliy(LinuxAddressFamily addressFamily)
        {
            return addressFamily == LinuxAddressFamily.AF_INET || addressFamily == LinuxAddressFamily.AF_INET6;
        }

        /// <summary>
        /// Check whether the given IP is in a local subnet
        /// For IPV4 - returns true if in subnet 127.0.0.0/24
        /// For IPV6 0 returns true only for the loopback address of ::1
        /// </summary>
        /// <param name="ip">The given ip to check, can be IPV4 or IPV6 format</param>
        /// <returns>True if the given ip is in local subnet</returns>
        public static bool IsLocalIp(string ip)
        {
            IPAddress address = IPAddress.Parse(ip);
            return IPAddress.IsLoopback(address);
        }
    }
}