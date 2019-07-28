// <copyright file="AggregatedProcessCreatedEventGeneratorUT.cs" company="Microsoft">
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
    /// 
    /// </summary>
    [TestClass]
    public class AggregatedProcessCreatedEventGeneratorUT : AggregatedEventGeneratorCommonUt<ProcessCreationPayload>
    {
        /// <inheritdoc />
        protected override AggregatedEventsGenerator<ProcessCreationPayload> CreateInstance(EventGenerator mockedEventGenerator)
        {
            return new AggregatedProcessCreateEventGeneratorCommon(mockedEventGenerator);
        }

        /// <inheritdoc />
        protected override EventBase<ProcessCreationPayload> E1 { get; } =
            new ProcessCreate(EventPriority.Low, new ProcessCreationPayload()
            {
                CommandLine = "cmd1",
                Executable = "exe1",
                ParentProcessId = 123,
                ProcessId = 12,
                UserId = "1",
                UserName = "user1",
                Time = DateTime.Now
            });

        /// <inheritdoc />
        protected override EventBase<ProcessCreationPayload> E2 { get; } =
            new ProcessCreate(EventPriority.Low, new ProcessCreationPayload()
            {
                CommandLine = "cmd2",
                Executable = "exe2",
                ParentProcessId = 223,
                ProcessId = 22,
                UserId = "2",
                UserName = "user2",
                Time = DateTime.Now
            });

        /// <inheritdoc />
        protected override bool ComparePayloads(ProcessCreationPayload p1, ProcessCreationPayload p2)
        {
            return p1.CommandLine == p2.CommandLine
                && p1.Executable == p2.Executable
                && p1.UserId == p2.UserId
                && p1.UserName == p2.UserName;
        }

        /// <inheritdoc />
        protected override void SetEventPriority(EventPriority priority)
        {
            Twin.ChangeEventPriority(typeof(ProcessCreate).Name, priority);
        }

        /// <inheritdoc />
        protected override void ToggleAggregation(bool swith)
        {
            Twin.ChangeConfiguration("aggregationEnabledProcessCreate", swith.ToString());
        }

        /// <inheritdoc />
        protected override void SetAggregationIntervalTime(string iso8601Interval)
        {
            Twin.ChangeConfiguration("aggregationIntervalProcessCreate", iso8601Interval);
        }
    }
}
