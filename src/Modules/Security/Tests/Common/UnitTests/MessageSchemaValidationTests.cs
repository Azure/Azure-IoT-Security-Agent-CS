// <copyright file="MessageSchemaValidationTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.Events;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;
using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Security.Tests.Common.Helpers;
using System;
using System.Collections.Generic;

namespace Security.Tests.Common.UnitTests
{
    /// <summary>
    /// Tests for message schema validation
    /// </summary>
    [TestClass]
    public class MessageSchemaValidationTests
    {
        /// <summary>
        /// Non empty ExtraDetails field
        /// </summary>
        [TestMethod]
        public void NonEmptyExtraDetails()
        {
            var payload = new DiagnosticPayload
            {
                CorrelationId = Guid.NewGuid(),
                Message = "message",
                ProcessId = 123,
                Severity = "Fatal",
                ThreadId = 432,
                ExtraDetails = new Dictionary<string, string>()
            };

            var obj = new Diagnostic(EventPriority.High, payload);
            obj.ValidateSchema();

            payload.ExtraDetails.Add("a", "b");
            obj = new Diagnostic(EventPriority.Operational, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="Diagnostic"/> event
        /// </summary>
        [TestMethod]
        public void Diagnostic()
        {
            var payload = new DiagnosticPayload
            {
                CorrelationId = Guid.NewGuid(),
                Message = "free-text",
                ProcessId = 123,
                ThreadId = 432,
            };

            foreach (var value in Enum.GetValues(typeof(LogLevel)))
            {
                if ((LogLevel)value == LogLevel.Off)
                    continue;

                payload.Severity = value.ToString();
                var obj = new Diagnostic(EventPriority.High, payload);
                obj.ValidateSchema();
            }
        }

        /// <summary>
        /// <see cref="ConfigurationError"/> event
        /// </summary>
        [TestMethod]
        public void ConfigurationError()
        {
            var payload = new ConfigurationErrorPayload
            {
                ConfigurationName = "config",
                Message = "some message",
                UsedConfiguration = "some value"
            };

            foreach (ConfigurationErrorType value in Enum.GetValues(typeof(ConfigurationErrorType)))
            {
                payload.ErrorType = value;
                var obj = new ConfigurationError(new ConfigurationErrorPayload[] { payload });
                obj.ValidateSchema();
            }
        }

        /// <summary>
        /// <see cref="DroppedEventsStatistics"/> event
        /// </summary>
        [TestMethod]
        public void DroppedEventsStatistics()
        {
            var payload = new DroppedEventsStatisticsPayload
            {
                CollectedEvents = 4,
                DroppedEvents = 5,
                Queue = EventPriority.Operational
            };

            foreach (EventPriority value in Enum.GetValues(typeof(EventPriority)))
            {
                if (value == EventPriority.Off || value == EventPriority.Operational)
                    continue;

                payload.Queue = value;
                var obj = new DroppedEventsStatistics(new DroppedEventsStatisticsPayload[] { payload });
                obj.ValidateSchema();
            }
        }

        /// <summary>
        /// <see cref="MessageStatistics"/> event
        /// </summary>
        [TestMethod]
        public void MessageStatistics()
        {
            var payload = new MessageStatisticsPayload
            {
                MessagesSent = 2,
                MessagesUnder4KB = 5,
                TotalFailed = 9
            };

            var obj = new MessageStatistics(new MessageStatisticsPayload[] { payload });
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="ConnectedHardware"/> event
        /// </summary>
        [TestMethod]
        [Ignore("Not supported")]
        public void ConnectedHardware()
        {
            var payload = new ConnectedHardwarePayload
            {
                ConnectedHardware = "floppy"
            };

            var obj = new ConnectedHardware(EventPriority.High, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="ListeningPorts"/> event
        /// </summary>
        [TestMethod]
        public void ListeningPorts()
        {
            var payload = new ListeningPortsPayload
            {
                LocalAddress = "::ffff:c000:0280",
                LocalPort = "234",
                Protocol = "tcp",
                RemoteAddress = "192.168.0.1",
                RemotePort = "32"
            };

            var obj = new ListeningPorts(EventPriority.Operational, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="ProcessCreate"/> event
        /// </summary>
        [TestMethod]
        public void ProcessCreate()
        {
            var payload = new ProcessCreationPayload
            {
                CommandLine = "c:\app.exe",
                Executable = "app.exe",
                ParentProcessId = 34,
                ProcessId = 56,
                Time = DateTime.UtcNow,
                UserId = "admin",
                UserName = "administrator"
            };

            var obj = new ProcessCreate(EventPriority.Low, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="ProcessTerminate"/> event
        /// </summary>
        [TestMethod]
        public void ProcessTerminate()
        {
            var payload = new ProcessTerminationPayload
            {
                Executable = "app.exe",
                Time = DateTime.UtcNow,
                ProcessId = 11,
                ExitStatus = 1
            };

            var obj = new ProcessTerminate(EventPriority.Low, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="SystemInformation"/> event
        /// </summary>
        [TestMethod]
        public void SystemInformation()
        {
            var payload = new SystemInformationPayload
            {
                BIOSVersion = "1.2.3",
                BootDevice = "x",
                FreePhysicalMemoryInKB = 123,
                FreeVirtualMemoryInKB = 44,
                HostName = "Bob",
                OsArchitecture = "x64",
                OSName = "Windows",
                OSVendor = "Microsoft",
                OSVersion = "3.11",
                TotalPhysicalMemoryInKB = 45,
                TotalVirtualMemoryInKB = 55
            };

            var obj = new SystemInformation(EventPriority.Operational, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="LocalUsers"/> event
        /// </summary>
        [TestMethod]
        public void LocalUsers()
        {
            var payload = new LocalUsersPayload
            {
                GroupIds = "9",
                GroupNames = "Administrators;Readers",
                UserId = "123",
                UserName = "admin"
            };

            var obj = new LocalUsers(EventPriority.Operational, payload);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="Login"/> event
        /// </summary>
        [TestMethod]
        public void Login()
        {
            var payload = new LoginPayload
            {
                Executable = "app.exe",
                UserName = "admin",
                UserId = "500",
                LocalPort = "900",
                Operation = "opX",
                ProcessId = 3,
                RemoteAddress = "10.0.0.1",
                RemotePort = "70",
                Result = LoginPayload.LoginResult.Fail
            };

            foreach (LoginPayload.LoginResult value in Enum.GetValues(typeof(LoginPayload.LoginResult)))
            {
                payload.Result = value;
                var obj = new Login(EventPriority.Low, payload, DateTime.UtcNow);
                obj.ValidateSchema();
            }
        }

        /// <summary>
        /// <see cref="ConnectionCreate"/> event
        /// </summary>
        [TestMethod]
        public void ConnectionCreate()
        {
            var payload = new ConnectionsPayload
            {
                RemotePort = "23",
                RemoteAddress = "192.168.0.1",
                ProcessId = 2,
                LocalPort = "8080",
                UserId = "111",
                CommandLine = "c:\\dir\\app.exe",
                Direction = ConnectionsPayload.ConnectionDirection.In,
                Executable = "app.exe",
                LocalAddress = "::ffff:c000:0280",
                Protocol = "tcp"
            };

            var obj = new ConnectionCreate(EventPriority.Low, payload, DateTime.UtcNow);
            obj.ValidateSchema();
        }

        /// <summary>
        /// <see cref="FirewallConfiguration"/> event
        /// </summary>
        [TestMethod]
        public void FirewallConfiguration()
        {
            var payload = new FirewallRulePayload
            {
                SourcePort = "41",
                Protocol = "udp",
                Application = "app",
                ChainName = "chain",
                DestinationAddress = "127.0.0.1",
                DestinationPort = "70",
                Enabled = false,
                SourceAddress = "10.0.0.1"
            };

            foreach (FirewallRulePayload.Directions value in Enum.GetValues(typeof(FirewallRulePayload.Directions)))
            {
                payload.Direction = value;
                var obj = new FirewallConfiguration(EventPriority.Low, payload);
                obj.ValidateSchema();
            }

            foreach (FirewallRulePayload.Actions  value in Enum.GetValues(typeof(FirewallRulePayload.Actions)))
            {
                payload.Action = value;
                var obj = new FirewallConfiguration(EventPriority.Low, payload);
                obj.ValidateSchema();
            }

            payload.Priority = 4;
            var withPriority = new FirewallConfiguration(EventPriority.Low, payload);
            withPriority.ValidateSchema();
        }

        /// <summary>
        /// <see cref="OSBaseline"/> event
        /// </summary>
        [TestMethod]
        public void OSBaseline()
        {
            var payload = new BaselinePayload
            {
                CceId = "1123",
                Description = "my description",
                Error = "error",
                Result = "Err",
                Severity = "Informational"
            };

            var obj = new OSBaseline(EventPriority.High, payload);
            obj.ValidateSchema();

            payload.Result = "Pass";
            obj = new OSBaseline(EventPriority.High, payload);
            obj.ValidateSchema();

            payload.Result = "Fail";
            obj = new OSBaseline(EventPriority.High, payload);
            obj.ValidateSchema();

            payload.Severity = "Critical";
            obj = new OSBaseline(EventPriority.High, payload);
            obj.ValidateSchema();

            payload.Severity = "Important";
            obj = new OSBaseline(EventPriority.High, payload);
            obj.ValidateSchema();

            payload.Severity = "Warning";
            obj = new OSBaseline(EventPriority.High, payload);
            obj.ValidateSchema();
        }
    }
}
