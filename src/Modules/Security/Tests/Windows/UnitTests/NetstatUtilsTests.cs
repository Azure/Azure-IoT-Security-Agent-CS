// <copyright file="NetstatUtilsTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.Common.Utils;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Security.Tests.Windows.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="NetstatUtils"/>
    /// </summary>
    [TestClass]
    public class NetstatUtilsTests
    {
        /// <summary>
        /// Tests for the <see cref="NetstatUtils.ParseNetstatListeners"/> method
        /// </summary>
        [TestClass]
        public class ParseNetstatListenersMethod : NetstatUtilsTests
        {
            private const int WindowsLocalAddressColumnNumber = 1;
            private const int WindowsRemoteAddressColumnNumber = 2;
            private const int LinuxLocalAddressColumnNumber = 3;
            private const int LinuxRemoteAddressColumnNumber = 4;

            /// <summary>
            /// IPV6 in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsIpv6()
            {
                const string Input = "  TCP    [2a01:110:68:2c:745a:5970:a2f2:e0f3]:64400  [2a02:26f0:e8:2b3::201a]:80  CLOSE_WAIT";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("2a01:110:68:2c:745a:5970:a2f2:e0f3", payload.LocalAddress);
                Assert.AreEqual("2a02:26f0:e8:2b3::201a", payload.RemoteAddress);
                Assert.AreEqual("64400", payload.LocalPort);
                Assert.AreEqual("80", payload.RemotePort);
            }

            /// <summary>
            /// IPv6 in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxTcpIpv6()
            {
                const string Input = "tcp6       0      0 2a01:110:68:2c:745a:5970:a2f2:e0f3:22                   2a02:26f0:e8:2b3::201a:*                    LISTEN";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, LinuxLocalAddressColumnNumber, LinuxRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp6", payload.Protocol);
                Assert.AreEqual("2a01:110:68:2c:745a:5970:a2f2:e0f3", payload.LocalAddress);
                Assert.AreEqual("2a02:26f0:e8:2b3::201a", payload.RemoteAddress);
                Assert.AreEqual("22", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
            }

            /// <summary>
            /// IPv4 in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxIpv4()
            {
                const string Input = "tcp        0      0 10.166.83.31:33872      10.166.83.16:631        TIME_WAIT";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, LinuxLocalAddressColumnNumber, LinuxRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("10.166.83.31", payload.LocalAddress);
                Assert.AreEqual("10.166.83.16", payload.RemoteAddress);
                Assert.AreEqual("33872", payload.LocalPort);
                Assert.AreEqual("631", payload.RemotePort);
            }

            /// <summary>
            /// IPv4 in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsIpv4()
            {
                const string Input = "  TCP    10.166.83.25:57604     192.30.253.124:443     ESTABLISHED";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("10.166.83.25", payload.LocalAddress);
                Assert.AreEqual("192.30.253.124", payload.RemoteAddress);
                Assert.AreEqual("57604", payload.LocalPort);
                Assert.AreEqual("443", payload.RemotePort);
            }

            /// <summary>
            /// IPv6 (Any) in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsIpv6Any()
            {
                const string Input = "  TCP    [::]:445               [::]:0                 LISTENING";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("445", payload.LocalPort);
                Assert.AreEqual("0", payload.RemotePort);
            }

            /// <summary>
            /// Unrecognized protocol should be ignored
            /// </summary>
            [TestMethod]
            public void ShouldIgnoreUnrecognizedProtocols()
            {
                const string Input = "  PrOtOcOl    10.166.83.25:57604     192.30.253.124:443     ESTABLISHED";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(0, payloads.Count);
            }

            /// <summary>
            /// Protocol name should be normalized to lowercase
            /// </summary>
            [TestMethod]
            public void ShouldNormalizeProtocolNameToLowercase()
            {
                const string Input = "  tCp    10.166.83.25:57604     192.30.253.124:443     ESTABLISHED";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
            }

            /// <summary>
            /// IPv6 (Any) in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxTcpIpv6Any()
            {
                const string Input = "tcp6       0      0 :::22                   :::*                    LISTEN";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, LinuxLocalAddressColumnNumber, LinuxRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("22", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
            }

            /// <summary>
            /// IPv6 (UDP) in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxUdpIpv6Any()
            {
                const string Input = "udp6       0      0 :::5353                 :::*                               ";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, LinuxLocalAddressColumnNumber, LinuxRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("udp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("5353", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
            }

            /// <summary>
            /// IPv4 in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsUdp()
            {
                const string Input = "  UDP    10.166.83.25:50049     *:*";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("10.166.83.25", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("50049", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
            }

            /// <summary>
            /// Combination in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsTcpUdpIpv6Ipv4Combination()
            {
                const string Input = @"  TCP    10.166.83.25:64758     23.64.31.74:443        ESTABLISHED
  TCP    [2001:0:2851:7871:28a9:6860:5823:3ae6]:7680  [2001:0:2851:7871:1054:f038:5823:3be2]:52540  TIME_WAIT
  UDP    0.0.0.0:123            *:*";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(3, payloads.Count);

                Assert.AreEqual("tcp", payloads.First().Protocol);
                Assert.AreEqual("10.166.83.25", payloads.First().LocalAddress);
                Assert.AreEqual("23.64.31.74", payloads.First().RemoteAddress);
                Assert.AreEqual("64758", payloads.First().LocalPort);
                Assert.AreEqual("443", payloads.First().RemotePort);

                Assert.AreEqual("tcp", payloads.Skip(1).First().Protocol);
                Assert.AreEqual("2001:0:2851:7871:28a9:6860:5823:3ae6", payloads.Skip(1).First().LocalAddress);
                Assert.AreEqual("2001:0:2851:7871:1054:f038:5823:3be2", payloads.Skip(1).First().RemoteAddress);
                Assert.AreEqual("7680", payloads.Skip(1).First().LocalPort);
                Assert.AreEqual("52540", payloads.Skip(1).First().RemotePort);

                Assert.AreEqual("udp", payloads.Skip(2).First().Protocol);
                Assert.AreEqual("0.0.0.0", payloads.Skip(2).First().LocalAddress);
                Assert.AreEqual("*", payloads.Skip(2).First().RemoteAddress);
                Assert.AreEqual("123", payloads.Skip(2).First().LocalPort);
                Assert.AreEqual("*", payloads.Skip(2).First().RemotePort);
            }
            
            /// <summary>
            /// netstat header in Windows
            /// </summary>
            [TestMethod]
            public void ShouldIgnoreWindowsHeader()
            {
                const string Input = @"
Active Connections

";

                List<ListeningPortsPayload> payload = NetstatUtils.ParseNetstatListeners(Input, WindowsLocalAddressColumnNumber, WindowsRemoteAddressColumnNumber);
                Assert.AreEqual(0, payload.Count);
            }
        }
    }
}
