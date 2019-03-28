// <copyright file="LinuxAddressFamily.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Connections
{
    /// <summary>
    /// The enum of the Linux Address Family name.
    /// This file is a copy of the header file of "linux/ socket.h"
    /// </summary>
    public enum LinuxAddressFamily
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        Unknown = -1,
        AF_UNSPEC = 0,
        AF_UNIX = 1,
        AF_LOCAL = 1,
        AF_INET = 2,
        AF_AX25 = 3,
        AF_IPX = 4,
        AF_APPLETALK = 5,
        AF_NETROM = 6,
        AF_BRIDGE = 7,
        AF_ATMPVC = 8,
        AF_X25 = 9,
        AF_INET6 = 10,
        AF_ROSE = 11,
        AF_DECnet = 12,
        AF_NETBEUI = 13,
        AF_SECURITY = 14,
        AF_KEY = 15,
        AF_NETLINK = 16,
        AF_ROUTE = 16,
        AF_PACKET = 17,
        AF_ASH = 18,
        AF_ECONET = 19,
        AF_ATMSVC = 20,
        AF_RDS = 21,
        AF_SNA = 22,
        AF_IRDA = 23,
        AF_PPPOX = 24,
        AF_WANPIPE = 25,
        AF_LLC = 26,
        AF_IB = 27,
        AF_MPLS = 28,
        AF_CAN = 29,
        AF_TIPC = 30,
        AF_BLUETOOTH = 31,
        AF_IUCV = 32,
        AF_RXRPC = 33,
        AF_ISDN = 34,
        AF_PHONET = 35,
        AF_IEEE802154 = 36,
        AF_CAIF = 37,
        AF_ALG = 38,
        AF_NFC = 39,
        AF_VSOCK = 40,
        AF_KCM = 41,
        AF_QIPCRTR = 42,
        AF_SMC = 43
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }

    /// <summary>
    /// Address families that are not relevant for the security log we collect
    /// </summary>
    public static class ExcludedFamilies
    {
        /// <summary>
        /// Returns families that are not relevant for the security log we collect
        /// </summary>
        /// <returns>True if the given LinuxAddressFamily should be ignored</returns>
        public static bool IsToIgnore(this LinuxAddressFamily addrFamily)
        {
            return addrFamily == LinuxAddressFamily.AF_UNSPEC ||
                   addrFamily == LinuxAddressFamily.AF_UNIX ||
                   addrFamily == LinuxAddressFamily.AF_LOCAL ||
                   addrFamily == LinuxAddressFamily.AF_KEY ||
                   addrFamily == LinuxAddressFamily.AF_NETLINK ||
                   addrFamily == LinuxAddressFamily.AF_ROUTE ||
                   addrFamily == LinuxAddressFamily.AF_ALG;
        }
    }
}