// <copyright file="AgentConfig.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Diagnostics;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.Collections.Specialized;
using Microsoft.Azure.IoT.Agent.Core.Utils;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers
{
    /// <summary>
    /// Holds the basic agent configuration 
    /// </summary>
    public class GeneralConfig
    {
        /// <summary>
        /// The fixed name of the AgentId key
        /// </summary>
        public const string AgentIdKey = "agentId";

        /// <summary>
        /// The fixed name of the ReadRemoteConfiguratioTimeout key
        /// </summary>
        public const string ReadRemoteConfigurationTimeoutKey = "readRemoteConfigurationTimeout";

        /// <summary>
        /// The fixed name of the SchedulerInternal key
        /// </summary>
        public const string SchedulerIntervalKey = "schedulerInterval";

        /// <summary>
        /// The fixed name of the ProducerInternal key
        /// </summary>
        public const string ProducerIntervalKey = "producerInterval";

        /// <summary>
        /// The fixed name of the ConsumerInterval key
        /// </summary>
        public const string ConsumerIntervalKey = "consumerInterval";

        /// <summary>
        /// The fixed name of the HighPriorityQueueSizePercentage key
        /// </summary>
        public const string HighPriorityQueueSizePercentageKey = "highPriorityQueueSizePercentage";

        /// <summary>
        /// The fixed name of the LogLevel key
        /// </summary>
        public const string LogLevelKey = "logLevel";

        /// <summary>
        /// The fixed name of the DiagnosticVerbosityLevel key
        /// </summary>
        public const string DiagnosticVerbosityLevelKey = "diagnosticVerbosityLevel";

        /// <summary>
        /// The fixed name of the LogFilePath key
        /// </summary>
        public const string LogFilePathKey = "logFilePath";

        /// <summary>
        /// The fixed name of the FefaultEventPriority key
        /// </summary>
        public const string DefaultEventPriorityKey = "defaultEventPriority";

        /// <summary>
        /// The fixed name of FileLogLevel key
        /// </summary>
        public const string FileLogLevelKey = "fileLogLevel";

        /// <summary>
        /// Gets the time to wait for remote configuration to load
        /// </summary>
        public TimeSpan ReadRemoteConfigurationTimeout { get; }

        /// <summary>
        /// Gets the interval between each scheduler executions
        /// </summary>
        public TimeSpan SchedulerInterval { get; }

        /// <summary>
        /// Gets the interval between each producer task execution
        /// </summary>
        public TimeSpan ProducerInterval { get; }

        /// <summary>
        /// Gets the interval between each consumer task execution
        /// </summary>
        public TimeSpan ConsumerInterval { get; }

        /// <summary>
        /// Gets the percentage that the high priority queue should occupy from the total alloted size of the queue
        /// </summary>
        public double HighPriorityQueueSizePercentage { get; }

        /// <summary>
        /// Gets the agent id
        /// </summary>
        public Guid AgentId { get; }

        /// <summary>
        /// Gets the log level of the agent
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        /// The level at which or above we send a diagnostic event from the logger
        /// </summary>
        public DiagnosticVerbosity DiagnosticVerbosityLevel { get; }

        /// <summary>
        /// Gets the path to the log file
        /// </summary>
        public string LogFilePath { get; }

        /// <summary>
        /// Gets the default event priority that will be used when not specified by the remote configuration
        /// </summary>
        public EventPriority DefaultEventPriority { get; }

        /// <summary>
        /// Gets the file log level of the agent
        /// </summary>
        public LogLevel FileLogLevel { get; }

        /// <summary>
        /// Gets the percentage that the low priority queue should occupy from the total alloted size of the queue
        /// </summary>
        public double LowPriorityQueueSizePercentage => 1 - HighPriorityQueueSizePercentage;

        /// <summary>
        /// Constructor that is initialized from a given configuration 
        /// </summary>
        /// <param name="nameValueCollection">nameValueCollection represents the user configuration</param>
        public GeneralConfig(NameValueCollection nameValueCollection)
        {
                FileLogLevel = nameValueCollection.GetEnumValueThrowOnFail<LogLevel>(FileLogLevelKey);
                ReadRemoteConfigurationTimeout = nameValueCollection.GetTimeSpanValueThrowOnFail(ReadRemoteConfigurationTimeoutKey);
                SchedulerInterval = nameValueCollection.GetTimeSpanValueThrowOnFail(SchedulerIntervalKey);
                ProducerInterval = nameValueCollection.GetTimeSpanValueThrowOnFail(ProducerIntervalKey);
                ConsumerInterval = nameValueCollection.GetTimeSpanValueThrowOnFail(ConsumerIntervalKey);
                HighPriorityQueueSizePercentage = nameValueCollection.GetDoubleValueThrowOnFail(HighPriorityQueueSizePercentageKey);
                AgentId = nameValueCollection.GetGuidValueThrowOnFail(AgentIdKey);
                LogLevel = nameValueCollection.GetEnumValueThrowOnFail<LogLevel>(LogLevelKey);
                DiagnosticVerbosityLevel = nameValueCollection.GetEnumValueThrowOnFail<DiagnosticVerbosity>(DiagnosticVerbosityLevelKey);
                LogFilePath = nameValueCollection.GetStringValueThrowOnFail(LogFilePathKey);
                DefaultEventPriority = nameValueCollection.GetEnumValueThrowOnFail<EventPriority>(DefaultEventPriorityKey);
        }
    }
}
