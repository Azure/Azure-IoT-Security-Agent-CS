// <copyright file="WindowsFirewallConfigurationGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Windows;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetFwTypeLib;
using Security.Tests.Common.Helpers;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using static Microsoft.Azure.Security.IoT.Contracts.Events.Payloads.FirewallRulePayload;

namespace Agent.Tests.Windows.UnitTests.EventGenerators
{
    /// <summary>
    /// Unit tests for firewall configuration event generator
    /// </summary>
    [TestCategory("WindowsOnly")]
    [TestClass]
    public class WindowsFirewallConfigurationGeneratorUT : UnitTestBase
    {
        private Mock<INetFwPolicy2> _policyMock;
        private Mock<INetFwMgr> _managerMock;

        private FirewallConfigurationEventGenerator _genertorUnderTest;

        /// <summary>
        /// Test init
        /// </summary>
        [TestInitialize]
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Verifies empty event is created if no rules are configured
        /// </summary>
        [TestMethod]
        public void TestEmptyPayloadIfNoRules()
        {
            SetupPolicy();
            var events = _genertorUnderTest.GetEvents();
            events.ValidateSchema();
            AssertEmptyEvent(events.First());
        }

        /// <summary>
        /// Verifes that rules are mapped correctly into payloads
        /// </summary>
        [TestMethod]
        public void TestPayloadIsCorrect()
        {
            SetupPolicy
            (
                GetRuleMock("RUleName1", "descrIptionOne", "appName1", "servicename1", (int)FirewallRuleProtocol.Any, "80", "3125", "1.0.0.127", "8.8.8.8", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, true, NET_FW_ACTION_.NET_FW_ACTION_BLOCK),
                GetRuleMock("RUleName2", "descrIptionTwo", "appName2", "servicename2", (int)FirewallRuleProtocol.Tcp, "345", "88", "1.0.0.126", "4.4.4.4", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, false, NET_FW_ACTION_.NET_FW_ACTION_ALLOW),
                GetRuleMock("RUleName3", "descrIptionThree", "appName3", "servicename3", (int)FirewallRuleProtocol.Udp, "345", "88", "1.0.0.126", "4.4.4.4", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, true, NET_FW_ACTION_.NET_FW_ACTION_MAX)
            );
            var events = _genertorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(3, ((FirewallConfiguration)events.First()).Payload.Count());

            var firstPayload = ((FirewallConfiguration)events.First()).Payload.ElementAt(0);
            
            Assert.AreEqual("appName1", firstPayload.Application);
            Assert.AreEqual("1.0.0.127", firstPayload.DestinationAddress);
            Assert.AreEqual("80", firstPayload.DestinationPort);
            Assert.AreEqual("8.8.8.8", firstPayload.SourceAddress);
            Assert.AreEqual("3125", firstPayload.SourcePort);
            Assert.AreEqual(Directions.In, firstPayload.Direction);
            Assert.AreEqual(true, firstPayload.Enabled);
            Assert.AreEqual(null, firstPayload.Priority);
            Assert.AreEqual("Any", firstPayload.Protocol);
            Assert.AreEqual(Actions.Deny, firstPayload.Action);

            var secondPayload = ((FirewallConfiguration)events.First()).Payload.ElementAt(1);

            Assert.AreEqual("appName2", secondPayload.Application);
            Assert.AreEqual("4.4.4.4", secondPayload.DestinationAddress);
            Assert.AreEqual("88", secondPayload.DestinationPort);
            Assert.AreEqual("1.0.0.126", secondPayload.SourceAddress);
            Assert.AreEqual("345", secondPayload.SourcePort);
            Assert.AreEqual(Directions.Out, secondPayload.Direction);
            Assert.AreEqual(false, secondPayload.Enabled);
            Assert.AreEqual(null, secondPayload.Priority);
            Assert.AreEqual("Tcp", secondPayload.Protocol);
            Assert.AreEqual(Actions.Allow, secondPayload.Action);

            var thirdPayload = ((FirewallConfiguration)events.First()).Payload.ElementAt(2);

            Assert.AreEqual("appName3", thirdPayload.Application);
            Assert.AreEqual("4.4.4.4", thirdPayload.DestinationAddress);
            Assert.AreEqual("88", thirdPayload.DestinationPort);
            Assert.AreEqual("1.0.0.126", thirdPayload.SourceAddress);
            Assert.AreEqual("345", thirdPayload.SourcePort);
            Assert.AreEqual(Directions.Out, thirdPayload.Direction);
            Assert.AreEqual(true, thirdPayload.Enabled);
            Assert.AreEqual(null, thirdPayload.Priority);
            Assert.AreEqual("Udp", thirdPayload.Protocol);
            Assert.AreEqual(Actions.Other, thirdPayload.Action);
        }

        /// <summary>
        /// Verifies that rules with unknown protocols are skipped and don't cause exceptions
        /// </summary>
        [TestMethod]
        public void TestUnknownProtocolRulesAreSkipped()
        {
            SetupPolicy
            (
                GetRuleMock("RUleName1", "descrIptionOne", "appName1", "servicename1", 43, "80", "3125", "1.0.0.127", "8.8.8.8", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN, true, NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
            );

            var events = _genertorUnderTest.GetEvents();
            events.ValidateSchema();

            AssertEmptyEvent(events.First());
        }

        /// <summary>
        /// Verifies that direction "Max" rules are skipped
        /// </summary>
        [TestMethod]
        public void TestDirectionMaxRulesAreSkipped()
        {
            SetupPolicy
            (
                GetRuleMock("RUleName1", "descrIptionOne", "appName1", "servicename1", (int)FirewallRuleProtocol.Tcp, "80", "3125", "1.0.0.127", "8.8.8.8", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_MAX, true, NET_FW_ACTION_.NET_FW_ACTION_BLOCK),
                GetRuleMock("RUleName2", "descrIptionTwo", "appName2", "servicename2", (int)FirewallRuleProtocol.Tcp, "345", "88", "4.4.4.4", "1.0.0.126", NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, false, NET_FW_ACTION_.NET_FW_ACTION_ALLOW)
            );

            var events = _genertorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(1, ((FirewallConfiguration)events.First()).Payload.Count());

            var firstPayload = ((FirewallConfiguration)events.First()).Payload.ElementAt(0);

            Assert.AreEqual("appName2", firstPayload.Application);
        }

        private void AssertEmptyEvent(IEvent ev)
        {
            Assert.AreEqual(0, ((FirewallConfiguration)ev).Payload.Count());
            Assert.IsTrue(ev.IsEmpty);
        }

        private void SetupPolicy(params Mock<INetFwRule>[] rules)
        {
            var rulesEnumerator = rules.GetEnumerator();
            int returnCode = 0;
            var enumeratorMock = new Mock<IEnumVARIANT>();
            enumeratorMock.Setup(em => em.Next(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<IntPtr>())).Callback(
                (int celt, object[] rgVar, IntPtr pceltFetched) =>
                {
                    returnCode = rulesEnumerator.MoveNext() ? 0 : 1;
                    rgVar[0] = ((Mock<INetFwRule>)rulesEnumerator.Current).Object;
                }).Returns(returnCode);


            var firewallRules = new Mock<INetFwRules>();
            firewallRules.SetupGet(fr => fr.Count).Returns(rules.Count());
            firewallRules.Setup(fr => fr.get__NewEnum()).Returns(enumeratorMock.Object);
            _policyMock = new Mock<INetFwPolicy2>();
            _policyMock.SetupGet(pm => pm.Rules).Returns(firewallRules.Object);
            _managerMock = new Mock<INetFwMgr>();
            _managerMock.SetupGet(pm => pm.LocalPolicy.CurrentProfile.FirewallEnabled).Returns(true);
            _genertorUnderTest = new FirewallConfigurationEventGenerator(_policyMock.Object, _managerMock.Object);
        }

        private Mock<INetFwRule> GetRuleMock(
            string name, 
            string description, 
            string applicationName, 
            string serviceName,
            int protocol,
            string localPorts,
            string remotePorts,
            string localAddresses,
            string remoteAddresses,
            NET_FW_RULE_DIRECTION_ direction,
            bool enabled,
            NET_FW_ACTION_ action )
        {
            var rule = new Mock<INetFwRule>();

            rule.SetupProperty(p => p.Name, name);
            rule.SetupProperty(p => p.Description, description);
            rule.SetupProperty(p => p.ApplicationName, applicationName);
            rule.SetupProperty(p => p.serviceName, serviceName);
            rule.SetupProperty(p => p.Protocol, protocol);
            rule.SetupProperty(p => p.LocalPorts, localPorts);
            rule.SetupProperty(p => p.LocalAddresses, localAddresses);
            rule.SetupProperty(p => p.RemotePorts, remotePorts);
            rule.SetupProperty(p => p.RemoteAddresses, remoteAddresses);
            rule.SetupProperty(p => p.Direction, direction);
            rule.SetupProperty(p => p.Enabled, enabled);
            rule.SetupProperty(p => p.Action, action);

            return rule;
        }
    }
}