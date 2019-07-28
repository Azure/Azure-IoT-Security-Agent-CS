// <copyright file="AggregatedConnectionCreateEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.Common.AggregatedEventGenerators;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.Security.IoT.Contracts.Events.Payloads;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Security.Tests.Common.UnitTests
{
    /// <summary>
    /// Test class for aggregated connection create event genertor
    /// </summary>
    [TestClass]
    public class AggregatedConnectionCreateEventGeneratorUT : AggregatedEventGeneratorCommonUt<ConnectionsPayload>
    {
        /// <inheritdoc />
        protected override EventBase<ConnectionsPayload> E1 { get; } =
            new ConnectionCreate(EventPriority.Low, new ConnectionsPayload
            {
                CommandLine = "cmd1",
                Direction = ConnectionsPayload.ConnectionDirection.In,
                Executable = "exe1",
                LocalAddress = "1.1.1.1",
                LocalPort = "11",
                ProcessId = 1,
                Protocol = "Tcp",
                RemoteAddress = "2.2.2.2",
                RemotePort = "22",
                UserId = "2"
            }, DateTime.Now);

        /// <inheritdoc />
        protected override EventBase<ConnectionsPayload> E2 { get; } =
            new ConnectionCreate(EventPriority.Low, new ConnectionsPayload
            {
                CommandLine = "cmd3",
                Direction = ConnectionsPayload.ConnectionDirection.In,
                Executable = "exe3",
                LocalAddress = "3.3.3.3",
                LocalPort = "33",
                ProcessId = 3,
                Protocol = "Tcp",
                RemoteAddress = "4.4.4.4",
                RemotePort = "44",
                UserId = "4"
            }, DateTime.Now);

        /// <inheritdoc />
        protected override bool ComparePayloads(ConnectionsPayload p1, ConnectionsPayload p2)
        {
            return p1.CommandLine == p2.CommandLine
                && p1.Direction == p2.Direction
                && p1.Executable == p2.Executable
                && p1.LocalAddress == p2.LocalAddress
                && p1.LocalPort == p2.LocalPort
                && p1.RemoteAddress == p2.RemoteAddress
                && p1.UserId == p2.UserId
                && (p1.Direction == ConnectionsPayload.ConnectionDirection.In && p1.LocalPort == p2.LocalPort) ||
                    (p1.Direction == ConnectionsPayload.ConnectionDirection.Out && p1.RemotePort == p2.RemotePort);
        }

        /// <inheritdoc />
        protected override AggregatedEventsGenerator<ConnectionsPayload> CreateInstance(EventGenerator mockedEventGenerator)
        {
            return new AggregatedConnectionCreateEventGeneratorCommon(mockedEventGenerator);
        }

        /// <inheritdoc />
        protected override void SetAggregationIntervalTime(string ISO8601Interval)
        {
            Twin.ChangeConfiguration("aggregationIntervalConnectionCreate", ISO8601Interval);
        }

        /// <inheritdoc />
        protected override void SetEventPriority(EventPriority priority)
        {
            Twin.ChangeEventPriority(nameof(ConnectionCreate), priority);
        }

        /// <inheritdoc />
        protected override void ToggleAggregation(bool swith)
        {
            Twin.ChangeConfiguration("aggregationEnabledConnectionCreate", swith.ToString());
        }
    }
}
