// <copyright file="AggregateProcessTerminateEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

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
    public class AggregateProcessTerminateEventGeneratorUT : AggregatedEventGeneratorCommonUt<ProcessTerminationPayload>
    {
        /// <inheritdoc />
        protected override EventBase<ProcessTerminationPayload> E1 { get; } =
            new ProcessTerminate(EventPriority.Low, new ProcessTerminationPayload
            {
                Executable = "exe1",
                ProcessId = 1,
                ExitStatus = 0
            });

        /// <inheritdoc />
        protected override EventBase<ProcessTerminationPayload> E2 { get; } =
            new ProcessTerminate(EventPriority.Low, new ProcessTerminationPayload
            {
                Executable = "exe2",
                ProcessId = 2,
                ExitStatus = 1
            });

        /// <inheritdoc />
        protected override bool ComparePayloads(ProcessTerminationPayload p1, ProcessTerminationPayload p2)
        {
            return p1.Executable == p2.Executable
                && p1.ExitStatus == p2.ExitStatus;
        }

        /// <inheritdoc />
        protected override AggregatedEventsGenerator<ProcessTerminationPayload> CreateInstance(EventGenerator mockedEventGenerator)
        {
            return new AggregatedProcessTerminateEventGeneratorCommon(mockedEventGenerator);
        }

        /// <inheritdoc />
        protected override void SetAggregationIntervalTime(string ISO8601Interval)
        {
            Twin.ChangeConfiguration("aggregationIntervalProcessTerminate", ISO8601Interval);
        }

        /// <inheritdoc />
        protected override void SetEventPriority(EventPriority priority)
        {
            Twin.ChangeEventPriority(nameof(ProcessTerminate), priority);
        }

        /// <inheritdoc />
        protected override void ToggleAggregation(bool swith)
        {
            Twin.ChangeConfiguration("aggregationEnabledProcessTerminate", swith.ToString());
        }
    }
}
