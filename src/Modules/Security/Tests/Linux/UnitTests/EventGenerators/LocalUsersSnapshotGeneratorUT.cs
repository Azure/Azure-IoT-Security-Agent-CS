// <copyright file="LocalUsersSnapshotGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Security.Tests.Common.Helpers;

namespace Tests.Linux.UnitTests.EventGenerators
{
    /// <summary>
    /// Unit tests for LocalUsersSnapshot event generator
    /// </summary>
    [TestClass]
    public class LocalUsersSnapshotGeneratorUT : UnitTestBase
    {
        private const string SimpleEtcPasswdOutput =
            @"root:x:0:0:root:/root:/bin/bash
daemon:x:1:1:daemon:/usr/sbin:/usr/sbin/nologin
bin:x:2:2:bin:/bin:/usr/sbin/nologin
sys:x:3:35:sys:/dev:/usr/sbin/nologin
t-amenoc:x:1000:1005:t-amenoc,,,:/home/t-amenoc:/bin/bash";


        private const string SimpleEtcGroupOutput =
            @"root:x:0:
daemon:x:1:
bin:x:2:
sys:x:35:
adm:x:4:syslog,t-amenoc:
t-amenoc:x:1005:";

        private LocalUsersSnapshotGenerator _genertorUnderTest;
        private Mock<IProcessUtil> _mockedShell;

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();

            Twin.ChangeEventPriority(nameof(LocalUsersSnapshotGenerator), EventPriority.High);

            _mockedShell = new Mock<IProcessUtil>();
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("passwd")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(SimpleEtcPasswdOutput);
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("group")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(SimpleEtcGroupOutput);

            _genertorUnderTest = new LocalUsersSnapshotGenerator(_mockedShell.Object);
        }

        /// <summary>
        /// Verify local users parsing
        /// </summary>
        [TestMethod]
        public void TestVerifyLocalUsersParsing()
        {
            var events = _genertorUnderTest.GetEvents().ToList();
            events.ValidateSchema();
            events.ForEach(ev => ev.ValidateSchema());

            Assert.AreEqual(1, events.Count);
            var payloads = events.Cast<LocalUsers>().SelectMany(ev => ev.Payload).ToList();

            Assert.AreEqual(5, payloads.Count);
            Assert.AreEqual(1, payloads.Count(p => p.UserId == "0" && p.UserName == "root" && p.GroupIds == "0" && p.GroupNames == "root"));
            Assert.AreEqual(1, payloads.Count(p => p.UserId == "1" && p.UserName == "daemon" && p.GroupIds == "1" && p.GroupNames == "daemon"));
            Assert.AreEqual(1, payloads.Count(p => p.UserId == "2" && p.UserName == "bin" && p.GroupIds == "2" && p.GroupNames == "bin"));
            Assert.AreEqual(1, payloads.Count(p => p.UserId == "3" && p.UserName == "sys" && p.GroupIds == "35" && p.GroupNames == "sys"));
            Assert.AreEqual(1, payloads.Count(p =>
                p.UserId == "1000" && p.UserName == "t-amenoc"
                                                   && p.GroupIds.Split(";").All(x => x == "1005" || x == "4")
                                                   && p.GroupNames.Split(";").All(x => x == "t-amenoc" || x == "adm")));
        }

        /// <summary>
        /// Verify local users parsing user with multiple group names sharing same gid
        /// </summary>
        [TestMethod]
        public void TestVerifyLocalUsersParsingWithMultipleGroupNamesSharingSameGid()
        {
            Mock<IProcessUtil> mockedShell = new Mock<IProcessUtil>();

            mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("passwd")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(@"user1:x:1002:1005::/home/user1:/bin/sh
user2:x:1003:1006::/home/user2:/bin/sh");
            mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("group")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(@"group1:x:1003:user1
group2:x:1004:user1
user1:x:1005:
group3:x:1003:
user2:x:1006:");

            LocalUsersSnapshotGenerator genertorUnderTest = new LocalUsersSnapshotGenerator(mockedShell.Object);

            var events = genertorUnderTest.GetEvents().ToList();
            events.ValidateSchema();
            events.ForEach(ev => ev.ValidateSchema());

            Assert.AreEqual(1, events.Count);

            var payloads = events.Cast<LocalUsers>().SelectMany(ev => ev.Payload).ToList();
            Assert.AreEqual(2, payloads.Count);
            Assert.AreEqual(1, payloads.Count(p => p.UserId == "1002" && p.UserName == "user1" && p.GroupIds == "1005;1003;1004" && p.GroupNames == "user1;group1;group3;group2"));
            Assert.AreEqual(1, payloads.Count(p => p.UserId == "1003" && p.UserName == "user2" && p.GroupIds == "1006" && p.GroupNames == "user2"));
        }
    }
}