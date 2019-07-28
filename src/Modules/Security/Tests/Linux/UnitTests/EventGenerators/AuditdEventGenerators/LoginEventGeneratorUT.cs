// <copyright file="LoginEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Agent.Tests.Linux.UnitTests.EventGenerators.AuditdEventGenerators;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.LoginPayload;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Security.Tests.Common.Helpers;

namespace Tests.Linux.UnitTests.EventGenerators.AuditdEventGenerators
{
    /// <summary>
    /// Unit tests for login event generator
    /// </summary>
    [TestClass]
    public class LoginEventGeneratorUT : AuditdUnitTestBase
    {
        private const string LoginEvent1 = "----time-> Mon Dec 24 10:55:33 2018 type=USER_AUTH msg=audit(1545641733.993:72): pid=1082 uid=0 auid=4294967295 ses=4294967295 msg='op=PAM:authentication acct=\"kfir\" exe=\"/usr/sbin/lightdm\" hostname=? addr=? terminal=:0 res=success'";

        /// <inheritdoc />
        public override AuditEventGeneratorBase CreateInstance(IProcessUtil processUtil)
        {
            return new UserLoginEventGenerator(processUtil);
        }

        /// <summary>
        /// Verifies no events are generated correctly
        /// </summary>
        [TestMethod]
        public void TestLoginEventParseing()
        {
            SetupAusearchReturnValue(LoginEvent1);

            var events = GeneratorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(1, events.Count());

            Login loginEvent = (Login)events.First();

            Assert.AreEqual("/usr/sbin/lightdm", loginEvent.Payload.First().Executable);
            Assert.AreEqual((uint)1082, loginEvent.Payload.First().ProcessId);
            Assert.AreEqual(null, loginEvent.Payload.First().UserId);
            Assert.AreEqual("kfir", loginEvent.Payload.First().UserName);
            Assert.AreEqual("PAM:authentication", loginEvent.Payload.First().Operation);
            Assert.AreEqual(LoginResult.Success, loginEvent.Payload.First().Result);
            Assert.AreEqual(null, loginEvent.Payload.First().RemoteAddress);

            MockedShell.VerifyAll();
        }
    }
}
