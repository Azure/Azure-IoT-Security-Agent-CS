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
            private const int WindowsPidColumnNumber = 4;
            private const int LinuxLocalAddressColumnNumber = 3;
            private const int LinuxRemoteAddressColumnNumber = 4;
            private const int LinuxPidColumnNumber = 6;

            /// <summary>
            /// Linux payload
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsPayload()
            {
                const string Input = @"

Active Connections

  Proto  Local Address          Foreign Address        State           PID
  TCP    0.0.0.0:80             0.0.0.0:0              LISTENING       4
  TCP    192.168.1.6:49750      10.166.83.164:22       SYN_SENT        25956
  TCP    192.168.1.6:49752      52.114.132.21:443      TIME_WAIT       0
  TCP    [::]:80                [::]:0                 LISTENING       4
  TCP    [::]:64868             [::]:0                 LISTENING       1056
  UDP    0.0.0.0:53             *:*                                    9364
  UDP    [::]:123               *:*                                    2088
  UDP    [::1]:53437            *:*                                    9060
  UDP    [fe80::f1f0:4b7:b5d:ff71%2]:53435  *:*                                    9060";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(9, payloads.Count);

                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("0.0.0.0", payload.LocalAddress);
                Assert.AreEqual("0.0.0.0", payload.RemoteAddress);
                Assert.AreEqual("80", payload.LocalPort);
                Assert.AreEqual("0", payload.RemotePort);
                Assert.AreEqual("4", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(1).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("192.168.1.6", payload.LocalAddress);
                Assert.AreEqual("10.166.83.164", payload.RemoteAddress);
                Assert.AreEqual("49750", payload.LocalPort);
                Assert.AreEqual("22", payload.RemotePort);
                Assert.AreEqual("25956", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(2).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("192.168.1.6", payload.LocalAddress);
                Assert.AreEqual("52.114.132.21", payload.RemoteAddress);
                Assert.AreEqual("49752", payload.LocalPort);
                Assert.AreEqual("443", payload.RemotePort);
                Assert.AreEqual("0", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(3).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("80", payload.LocalPort);
                Assert.AreEqual("0", payload.RemotePort);
                Assert.AreEqual("4", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(4).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("64868", payload.LocalPort);
                Assert.AreEqual("0", payload.RemotePort);
                Assert.AreEqual("1056", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(5).First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("0.0.0.0", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("53", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("9364", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(6).First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("123", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("2088", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(7).First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("::1", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("53437", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("9060", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(8).First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("fe80::f1f0:4b7:b5d:ff71%2", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("53435", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("9060", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// Linux payload
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxPayload()
            {
                const string Input = @"Proto Recv-Q Send-Q Local Address           Foreign Address         State       PID/Program name
tcp        0      0 127.0.0.53:53           0.0.0.0:*               LISTEN      -
tcp6       0      0 :::39565                :::*                    LISTEN      8023/node
tcp6       0      0 :::22                   :::*                    LISTEN      -
udp        0      0 0.0.0.0:631             0.0.0.0:*                           -
udp6       0      0 :::5353                 :::*                                -
raw6       0      0 :::58                   :::*                    7           -                   ";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    LinuxLocalAddressColumnNumber,
                    LinuxRemoteAddressColumnNumber,
                    LinuxPidColumnNumber
                );

                Assert.AreEqual(6, payloads.Count);

                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("127.0.0.53", payload.LocalAddress);
                Assert.AreEqual("0.0.0.0", payload.RemoteAddress);
                Assert.AreEqual("53", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.IsNull(payload.ExtraDetails);

                payload = payloads.Skip(1).First();
                Assert.AreEqual("tcp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("39565", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("8023", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(2).First();
                Assert.AreEqual("tcp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("22", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.IsNull(payload.ExtraDetails);

                payload = payloads.Skip(3).First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("0.0.0.0", payload.LocalAddress);
                Assert.AreEqual("0.0.0.0", payload.RemoteAddress);
                Assert.AreEqual("631", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.IsNull(payload.ExtraDetails);

                payload = payloads.Skip(4).First();
                Assert.AreEqual("udp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("5353", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.IsNull(payload.ExtraDetails);

                payload = payloads.Skip(5).First();
                Assert.AreEqual("raw6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("58", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.IsNull(payload.ExtraDetails);
            }

            /// <summary>
            /// IPV6 in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsIpv6()
            {
                const string Input = "  TCP    [2a01:110:68:2c:745a:5970:a2f2:e0f3]:64400  [2a02:26f0:e8:2b3::201a]:80  CLOSE_WAIT     1234";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("2a01:110:68:2c:745a:5970:a2f2:e0f3", payload.LocalAddress);
                Assert.AreEqual("2a02:26f0:e8:2b3::201a", payload.RemoteAddress);
                Assert.AreEqual("64400", payload.LocalPort);
                Assert.AreEqual("80", payload.RemotePort);
                Assert.AreEqual("1234", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// IPv6 in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxTcpIpv6()
            {
                const string Input = "tcp6       0      0 2a01:110:68:2c:745a:5970:a2f2:e0f3:22                   2a02:26f0:e8:2b3::201a:*                    LISTEN      8023/node";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    LinuxLocalAddressColumnNumber,
                    LinuxRemoteAddressColumnNumber,
                    LinuxPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp6", payload.Protocol);
                Assert.AreEqual("2a01:110:68:2c:745a:5970:a2f2:e0f3", payload.LocalAddress);
                Assert.AreEqual("2a02:26f0:e8:2b3::201a", payload.RemoteAddress);
                Assert.AreEqual("22", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("8023", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// IPv4 in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxIpv4()
            {
                const string Input = "tcp        0      0 10.166.83.31:33872      10.166.83.16:631        TIME_WAIT      8023/node";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    LinuxLocalAddressColumnNumber,
                    LinuxRemoteAddressColumnNumber,
                    LinuxPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("10.166.83.31", payload.LocalAddress);
                Assert.AreEqual("10.166.83.16", payload.RemoteAddress);
                Assert.AreEqual("33872", payload.LocalPort);
                Assert.AreEqual("631", payload.RemotePort);
                Assert.AreEqual("8023", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// IPv4 in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsIpv4()
            {
                const string Input = "  TCP    10.166.83.25:57604     192.30.253.124:443     ESTABLISHED       1234";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("10.166.83.25", payload.LocalAddress);
                Assert.AreEqual("192.30.253.124", payload.RemoteAddress);
                Assert.AreEqual("57604", payload.LocalPort);
                Assert.AreEqual("443", payload.RemotePort);
                Assert.AreEqual("1234", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// IPv6 (Any) in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsIpv6Any()
            {
                const string Input = "  TCP    [::]:445               [::]:0                 LISTENING       1234";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("445", payload.LocalPort);
                Assert.AreEqual("0", payload.RemotePort);
                Assert.AreEqual("1234", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// Unrecognized protocol should be ignored
            /// </summary>
            [TestMethod]
            public void ShouldIgnoreUnrecognizedProtocols()
            {
                const string Input = "  PrOtOcOl    10.166.83.25:57604     192.30.253.124:443     ESTABLISHED       1234";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(0, payloads.Count);
            }

            /// <summary>
            /// Protocol name should be normalized to lowercase
            /// </summary>
            [TestMethod]
            public void ShouldNormalizeProtocolNameToLowercase()
            {
                const string Input = "  tCp    10.166.83.25:57604     192.30.253.124:443     ESTABLISHED       1234";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

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
                const string Input = "tcp6       0      0 :::22                   :::*                    LISTEN       8023/node";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    LinuxLocalAddressColumnNumber,
                    LinuxRemoteAddressColumnNumber,
                    LinuxPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("22", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("8023", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// IPv6 (UDP) in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxUdpIpv6Any()
            {
                const string Input = "udp6       0      0 :::5353                 :::*                               8023/node";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    LinuxLocalAddressColumnNumber,
                    LinuxRemoteAddressColumnNumber,
                    LinuxPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("udp6", payload.Protocol);
                Assert.AreEqual("::", payload.LocalAddress);
                Assert.AreEqual("::", payload.RemoteAddress);
                Assert.AreEqual("5353", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual(1, payload.ExtraDetails.Count());
                Assert.AreEqual("8023", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// Pid in Linux
            /// </summary>
            [TestMethod]
            public void ShouldParseLinuxPid()
            {
                const string Input = @"tcp       0      0 1.2.3.4:1                4.3.2.1:1                    LISTEN      1234/node
tcp       0      0 4.3.2.1:2                   1.2.3.4:2                    LISTEN      -";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    LinuxLocalAddressColumnNumber,
                    LinuxRemoteAddressColumnNumber,
                    LinuxPidColumnNumber
                );

                Assert.AreEqual(2, payloads.Count);

                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("1.2.3.4", payload.LocalAddress);
                Assert.AreEqual("4.3.2.1", payload.RemoteAddress);
                Assert.AreEqual("1", payload.LocalPort);
                Assert.AreEqual("1", payload.RemotePort);
                Assert.AreEqual("1234", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(1).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("4.3.2.1", payload.LocalAddress);
                Assert.AreEqual("1.2.3.4", payload.RemoteAddress);
                Assert.AreEqual("2", payload.LocalPort);
                Assert.AreEqual("2", payload.RemotePort);
                Assert.IsNull(payload.ExtraDetails);
            }

             /// <summary>
            /// Pid in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsPid()
            {
                const string Input = @"  TCP    192.168.1.6:49729      40.103.33.102:443      ESTABLISHED     13324
  TCP    192.168.1.6:49732      23.97.157.56:443       TIME_WAIT       0";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(2, payloads.Count);

                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("192.168.1.6", payload.LocalAddress);
                Assert.AreEqual("40.103.33.102", payload.RemoteAddress);
                Assert.AreEqual("49729", payload.LocalPort);
                Assert.AreEqual("443", payload.RemotePort);
                Assert.AreEqual("13324", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(1).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("192.168.1.6", payload.LocalAddress);
                Assert.AreEqual("23.97.157.56", payload.RemoteAddress);
                Assert.AreEqual("49732", payload.LocalPort);
                Assert.AreEqual("443", payload.RemotePort);
                Assert.AreEqual("0", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// IPv4 in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsUdp()
            {
                const string Input = "  UDP    10.166.83.25:50049     *:*                               1234";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(1, payloads.Count);
                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("10.166.83.25", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("50049", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("1234", payload.ExtraDetails["Pid"]);
            }

            /// <summary>
            /// Combination in Windows
            /// </summary>
            [TestMethod]
            public void ShouldParseWindowsTcpUdpIpv6Ipv4Combination()
            {
                const string Input = @"  TCP    10.166.83.25:64758     23.64.31.74:443        ESTABLISHED       1234
  TCP    [2001:0:2851:7871:28a9:6860:5823:3ae6]:7680  [2001:0:2851:7871:1054:f038:5823:3be2]:52540  TIME_WAIT       2345
  UDP    0.0.0.0:123            *:*                                    3456";

                List<ListeningPortsPayload> payloads = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(3, payloads.Count);

                ListeningPortsPayload payload = payloads.First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("10.166.83.25", payload.LocalAddress);
                Assert.AreEqual("23.64.31.74", payload.RemoteAddress);
                Assert.AreEqual("64758", payload.LocalPort);
                Assert.AreEqual("443", payload.RemotePort);
                Assert.AreEqual("1234", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(1).First();
                Assert.AreEqual("tcp", payload.Protocol);
                Assert.AreEqual("2001:0:2851:7871:28a9:6860:5823:3ae6", payload.LocalAddress);
                Assert.AreEqual("2001:0:2851:7871:1054:f038:5823:3be2", payload.RemoteAddress);
                Assert.AreEqual("7680", payload.LocalPort);
                Assert.AreEqual("52540", payload.RemotePort);
                Assert.AreEqual("2345", payload.ExtraDetails["Pid"]);

                payload = payloads.Skip(2).First();
                Assert.AreEqual("udp", payload.Protocol);
                Assert.AreEqual("0.0.0.0", payload.LocalAddress);
                Assert.AreEqual("*", payload.RemoteAddress);
                Assert.AreEqual("123", payload.LocalPort);
                Assert.AreEqual("*", payload.RemotePort);
                Assert.AreEqual("3456", payload.ExtraDetails["Pid"]);
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

                List<ListeningPortsPayload> payload = NetstatUtils.ParseNetstatListeners(
                    Input,
                    WindowsLocalAddressColumnNumber,
                    WindowsRemoteAddressColumnNumber,
                    WindowsPidColumnNumber
                );

                Assert.AreEqual(0, payload.Count);
            }
        }
    }
}
