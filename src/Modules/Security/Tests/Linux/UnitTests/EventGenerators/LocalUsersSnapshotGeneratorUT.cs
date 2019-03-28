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
        private const string Passwd =
            @"root:x:0:0:root:/root:/bin/bash
daemon:x:1:1:daemon:/usr/sbin:/usr/sbin/nologin
bin:x:2:2:bin:/bin:/usr/sbin/nologin
sys:x:3:35:sys:/dev:/usr/sbin/nologin
t-amenoc:x:1000:1005:t-amenoc,,,:/home/t-amenoc:/bin/bash";

        
        private const string Group =
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
                .Returns(Passwd);
            _mockedShell.Setup(m => m.ExecuteProcess(
                    It.IsAny<string>(),
                    It.Is<string>(x => x.Contains("group")),
                    It.IsAny<ErrorHandler>(),
                    It.IsAny<IEnumerable<int>>()))
                .Returns(Group);

            _genertorUnderTest = new LocalUsersSnapshotGenerator(_mockedShell.Object);
        }

        /// <summary>
        /// Verify local users parsing
        /// </summary>
        [TestMethod]
        public void VerifyLocalUsersParsing()
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
    }
}