// <copyright file="NetStatEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Security.Tests.Common.Helpers;

namespace Agent.Tests.Linux.UnitTests.EventGenerators
{
    /// <summary>
    /// Unit tests for netstat event generator
    /// </summary>
    [TestClass]
    public class NetStatEventGeneratorUT : UnitTestBase
    {
        private const string NetStatNoListening =
            @"Active Internet connections (only servers)
Proto Recv-Q Send-Q Local Address           Foreign Address         State       PID/Program name
Active UNIX domain sockets (only servers)
Proto RefCnt Flags       Type       State         I-Node   PID/Program name    Path
unix  2      [ ACC ]     STREAM     LISTENING     21556    2106/gnome-session- @/tmp/.ICE-unix/2106
unix  2      [ ACC ]     STREAM     LISTENING     18906    1539/systemd        /run/user/1000/systemd/private
unix  2      [ ACC ]     SEQPACKET  LISTENING     9375     1/init              /run/udev/control
unix  2      [ ACC ]     STREAM     LISTENING     18927    1651/gnome-keyring- /run/user/1000/keyring/control
unix  2      [ ACC ]     STREAM     LISTENING     19483    1651/gnome-keyring- /run/user/1000/keyring/pkcs11
unix  2      [ ACC ]     STREAM     LISTENING     19486    1651/gnome-keyring- /run/user/1000/keyring/ssh
unix  2      [ ACC ]     STREAM     LISTENING     9220     1/init              /run/systemd/private
unix  2      [ ACC ]     STREAM     LISTENING     9224     1/init              /run/systemd/journal/stdout";

        private const string NetStat =
            @"Active Internet connections (only servers)
Proto Recv-Q Send-Q Local Address           Foreign Address         State       PID/Program name
tcp        0      0 0.0.0.0:999             0.0.0.0:*               LISTEN      29183/nc        
tcp6       0      0 :::9900                 :::*                    LISTEN      31106/nc        
tcp6       0      0 ::1:990                 :::*                    LISTEN      31222/nc        
udp        0      0 0.0.0.0:888             0.0.0.0:*                           29205/nc        
udp        0      0 0.0.0.0:888             0.0.0.0:*                           29141/nc        
udp        0      0 0.0.0.0:5353            0.0.0.0:*                           1951/avahi-daemon: 
udp        0      0 0.0.0.0:55902           0.0.0.0:*                           1951/avahi-daemon: 
udp        0      0 0.0.0.0:68              0.0.0.0:*                           27974/dhclient  
udp        0      0 0.0.0.0:631             0.0.0.0:*                           2073/cups-browsed
udp6       0      0 :::5353                 :::*                                1951/avahi-daemon: 
udp6       0      0 :::54592                :::*                                1951/avahi-daemon: 
raw6       0      0 :::58                   :::*                    7           1979/NetworkManager
";

        private NetstatEventGenerator _genertorUnderTest;
        private Mock<IProcessUtil> _mockedShell;

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
            Twin.ChangeEventPriority(nameof(NetstatEventGenerator), EventPriority.High);
            _mockedShell = new Mock<IProcessUtil>();
            _genertorUnderTest = new NetstatEventGenerator(_mockedShell.Object);
        }

        /// <summary>
        /// Verify empty event if netstat returns no listening ports
        /// </summary>
        [TestMethod]
        public void VerifyNetstatNoListeningPortsEvent()
        {
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("netstat")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(NetStatNoListening);

            var events = _genertorUnderTest.GetEvents().ToList();
            events.ValidateSchema();
            events.ForEach(ev => ev.ValidateSchema());

            Assert.AreEqual(1, events.Count);
            var payloads = events.Cast<ListeningPorts>().SelectMany(ev => ev.Payload).ToList();
            Assert.AreEqual(0, payloads.Count);
            Assert.IsTrue(events[0].IsEmpty);
        }

        /// <summary>
        /// Verify netstat is parsed correctly
        /// </summary>
        [TestMethod]
        public void VerifyNetstatParsing()
        { 
            var a = IPAddress.Parse("::");
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("netstat")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(NetStat);

            var events = _genertorUnderTest.GetEvents().ToList();
            events.ValidateSchema();
            events.ForEach(ev => ev.ValidateSchema());

            Assert.AreEqual(1, events.Count);
            var payloads = events.Cast<ListeningPorts>().SelectMany(ev => ev.Payload).ToList();
            Assert.AreEqual(12, payloads.Count);
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "0.0.0.0" && p.LocalPort == "999" && p.Protocol == "tcp" && p.RemoteAddress == "0.0.0.0" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "::" && p.LocalPort == "9900" && p.Protocol == "tcp6" && p.RemoteAddress == "::" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "::1" && p.LocalPort == "990" && p.Protocol == "tcp6" && p.RemoteAddress == "::" && p.RemotePort == "*"));
            Assert.AreEqual(2, payloads.Count(p => p.LocalAddress == "0.0.0.0" && p.LocalPort == "888" && p.Protocol == "udp" && p.RemoteAddress == "0.0.0.0" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "0.0.0.0" && p.LocalPort == "5353" && p.Protocol == "udp" && p.RemoteAddress == "0.0.0.0" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "0.0.0.0" && p.LocalPort == "55902" && p.Protocol == "udp" && p.RemoteAddress == "0.0.0.0" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "0.0.0.0" && p.LocalPort == "68" && p.Protocol == "udp" && p.RemoteAddress == "0.0.0.0" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "0.0.0.0" && p.LocalPort == "631" && p.Protocol == "udp" && p.RemoteAddress == "0.0.0.0" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "::" && p.LocalPort == "5353" && p.Protocol == "udp6" && p.RemoteAddress == "::" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "::" && p.LocalPort == "54592" && p.Protocol == "udp6" && p.RemoteAddress == "::" && p.RemotePort == "*"));
            Assert.AreEqual(1, payloads.Count(p => p.LocalAddress == "::" && p.LocalPort == "58" && p.Protocol == "raw6" && p.RemoteAddress == "::" && p.RemotePort == "*"));
        }
    }
}