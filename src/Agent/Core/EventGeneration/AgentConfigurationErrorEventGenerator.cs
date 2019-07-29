// <copyright file="AgentConfigurationErrorEventGenerator.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents;
using Microsoft.Azure.IoT.Contracts.Events.OperationalEvents.Payloads;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{

    delegate bool Validator(RemoteConfiguration remoteConfiguration, out ConfigurationErrorPayload output);
    
    /// <summary>
    /// Operational event generator for configuration error evens
    /// </summary>
    public class AgentConfigurationErrorEventGenerator : EventGenerator
    {
        private readonly ConcurrentBag<ConfigurationError> _events = new ConcurrentBag<ConfigurationError>();

        private readonly List<Validator> _validators = new List<Validator>()
        {
            ValidateMaxMessageSize,
            ValidateMaxCacheSize,
            ValidateIntervals
        };

        /// <summary>
        /// C-tor
        /// </summary>
        public AgentConfigurationErrorEventGenerator()
        {
            RemoteConfiguration remoteConfiguration = AgentConfiguration.RemoteConfiguration;
            ValidatAgentConfiguration(remoteConfiguration);
            AgentConfiguration.AgentConfigurationChanged += ValidatAgentConfiguration;
            ExternalInterfaceFacade.Instance.RemoteConfigurationProvider.FailedToParseConfiguration += CreateTypeMismatchEvent;
        }

        /// <inheritdoc />
        public override EventPriority Priority => EventPriority.Operational;

        /// <inheritdoc />
        public override IEnumerable<IEvent> GetEvents()
        {
            var events = new List<IEvent>();
            while (_events.TryTake(out var ev))
            {
                events.Add(ev);
            }

            return events;
        }

        /// <summary>
        /// Event handler for OnFailedToParseConfiguration
        /// </summary>
        /// <param name="mismatchedTypeConfigurations">failed to parse configurations</param>
        private void CreateTypeMismatchEvent(IList<string> mismatchedTypeConfigurations)
        {
            //Remove previous events, they are no longer relevant
            RemoveEvents();

            var ev = new ConfigurationError(new[]
                {
                    new ConfigurationErrorPayload
                    {
                        ErrorType = ConfigurationErrorType.TypeMismatch,
                        ConfigurationName = nameof(RemoteConfiguration),
                        UsedConfiguration = AgentConfiguration.RemoteConfiguration.ToString(),
                        Message = $"Couldn't parse the following configurations: {string.Join(", ", mismatchedTypeConfigurations)}"
                    }
                }
            );

            _events.Add(ev);
        }

        /// <summary>
        /// Validates agent configuration make sense
        /// </summary>
        private void ValidatAgentConfiguration(RemoteConfiguration remoteConfiguration)
        {
            //Remove previous events, they are no longer relevant
            RemoveEvents();

            var payloads = new List<ConfigurationErrorPayload>();
            foreach (Validator validator in _validators)
            {
                if (validator(remoteConfiguration, out ConfigurationErrorPayload confErrorPayload) == false)
                {
                    payloads.Add(confErrorPayload);
                }
                
            }

            if (payloads.Count!= 0)
            {
                _events.Add(new ConfigurationError(payloads.ToArray()));
            }
        }

        /// <summary>
        /// Removes all of the accumulated events
        /// </summary>
        private void RemoveEvents()
        {
            while(!_events.IsEmpty)
            {
                _events.TryTake(out ConfigurationError placeholder);
            }
        }

        /// <summary>
        /// Validate maxMessageSize is multiple of 4Kb
        /// </summary>
        /// <param name="remoteConfiguration">Configuration to validate</param>
        /// <param name="configurationErrorPayload">configuration error payload</param>
        /// <returns>true if valid</returns>
        private static bool ValidateMaxMessageSize(RemoteConfiguration remoteConfiguration, out ConfigurationErrorPayload configurationErrorPayload)
        {
            configurationErrorPayload = null;
            if (remoteConfiguration.MaxMessageSize % Constants.OptimalMessageSizeMultipleInKb != 0)
            {
                configurationErrorPayload = new ConfigurationErrorPayload
                {
                    ConfigurationName = JsonUtils.GetJsonLabel(nameof(remoteConfiguration.MaxMessageSize), typeof(RemoteConfiguration)),
                    ErrorType = ConfigurationErrorType.NotOptimal,
                    UsedConfiguration = AgentConfiguration.RemoteConfiguration.MaxMessageSize.ToString(),
                    Message = $"Message size should be multiple of {Constants.OptimalMessageSizeMultipleInKb}"
                };

                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate maxCacheSize > maxMessageSize
        /// </summary>
        /// <param name="remoteConfiguration">Configuration to validate</param>
        /// <param name="configurationErrorPayload">configurationErrorPayload</param>
        /// <returns>true if valid</returns>
        private static bool ValidateMaxCacheSize(RemoteConfiguration remoteConfiguration, out ConfigurationErrorPayload configurationErrorPayload)
        {
            configurationErrorPayload = null;
            if (remoteConfiguration.MaxMessageSize > remoteConfiguration.MaxLocalCacheSize)
            {
                configurationErrorPayload = new ConfigurationErrorPayload
                {
                    ConfigurationName = JsonUtils.GetJsonLabel(nameof(remoteConfiguration.MaxLocalCacheSize), typeof(RemoteConfiguration)),
                    ErrorType = ConfigurationErrorType.Conflict,
                    UsedConfiguration = AgentConfiguration.RemoteConfiguration.MaxLocalCacheSize.ToString(),
                    Message = $"MaxLocalCacheSize is smaller than MaxMessageSize"
                };

                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate HighPriorityIntervals is lower or equal to LowPriorityInterval
        /// </summary>
        /// <param name="remoteConfiguration">Configuration to validate</param>
        /// <param name="configurationErrorPayload"></param>
        /// <returns>true if valid</returns>
        private static bool ValidateIntervals(RemoteConfiguration remoteConfiguration, out ConfigurationErrorPayload configurationErrorPayload)
        {
            configurationErrorPayload = null;
            if (remoteConfiguration.HighPriorityMessageFrequency > remoteConfiguration.LowPriorityMessageFrequency)
            {
                configurationErrorPayload = new ConfigurationErrorPayload
                {
                    ConfigurationName = JsonUtils.GetJsonLabel(nameof(remoteConfiguration.HighPriorityMessageFrequency), typeof(RemoteConfiguration)),
                    ErrorType = ConfigurationErrorType.Conflict,
                    UsedConfiguration = AgentConfiguration.RemoteConfiguration.HighPriorityMessageFrequency.ToString(),
                    Message = $"High priority interval is longer than low priority interval"
                };

                return false;
            }

            return true;
        }
    }
}