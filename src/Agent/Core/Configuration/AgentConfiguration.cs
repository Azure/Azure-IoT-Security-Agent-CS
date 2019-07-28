// <copyright file="AgentConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients;
using Microsoft.Azure.IoT.Contracts.Events;
using System;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration
{
    /// <summary>
    /// Agent configuration changed event
    /// </summary>
    public delegate void AgentConfigutationChanged(RemoteConfiguration remoteConfiguration);

    /// <summary>
    /// This class loads the agent configuration (not including local conf reads from a local file - TBD).
    /// The configuration are hard-coded and can be overwritten by the remote configuration
    /// Singleton implementation.
    /// Init() MUST be called before any other method is called (throwing Exception otherwise)
    /// </summary>
    public sealed class AgentConfiguration
    {
        /// <summary>
        /// Event handlers for AgentConfigurationChanged
        /// </summary>
        public static event AgentConfigutationChanged AgentConfigurationChanged;

        /// <summary>
        /// The class that holds the configuration with their default values
        /// </summary>
        private static RemoteConfiguration _remoteConfiguration;

        //False by default
        private static bool _isInitialized;

        /// <summary>
        /// Loads the initial configuration of the agent.
        /// This function should be called once when the agent starts.
        /// </summary>
        public static void Init()
        {
            ExternalInterfaceFacade.Instance.RemoteConfigurationProvider.RemoteConfigurationChanged += OnRemoteConfigurationProviderChange;

            RemoteConfiguration remoteConfiguration = ExternalInterfaceFacade.Instance.RemoteConfigurationProvider.GetRemoteConfigurationData();
            _remoteConfiguration = remoteConfiguration;

            _isInitialized = true;
            OnAgentConfigurationChanged(remoteConfiguration);
        }

        private static void OnRemoteConfigurationProviderChange(RemoteConfiguration newConfiguration)
        {
            RemoteConfiguration = newConfiguration;
        }

        /// <summary>
        /// Returns true if the configuration were initialized
        /// </summary>
        /// <returns>bool _isInitialized </returns>
        public static bool IsInitialized()
        {
            return _isInitialized;
        }

        /// <summary>
        /// Return the configurations from the remote configuration or their default values if they are not included in it
        /// </summary>
        /// <returns><see cref="RemoteConfiguration"/> object</returns>
        public static RemoteConfiguration RemoteConfiguration
        {
            get
            {
                EnsureInitialized();
                return _remoteConfiguration;
            }
            set
            {
                RemoteConfiguration remoteConfiguration = value ?? throw new ArgumentNullException(nameof(value));
                _remoteConfiguration = remoteConfiguration;
                OnAgentConfigurationChanged(remoteConfiguration);
            }
        }

        /// <summary>
        /// Returns the priority of the event
        /// The priority is determine by a default value but can be overwritten by the remote configuration
        /// </summary>
        /// <typeparam name="TEvent">The name of the event</typeparam>
        /// <returns>EventPriority</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case the event name is not defined in EventPrioritiesConfiguration</exception>
        /// <exception cref="InvalidOperationException">In case the method was called before Init() was called - should not happen in a normal flow of the agent</exception>
        public static EventPriority GetEventPriority<TEvent>() where TEvent : IEvent
        {
            EnsureInitialized();
            //Get the event property
            var eventType = _remoteConfiguration.GetType().GetProperty(typeof(TEvent).Name);
            if (eventType == null)
            {
                throw new ArgumentOutOfRangeException(paramName: $"{nameof(EventPriority)}",
                    message: $"TwinConfiguration error: Couldn't find priority configuration for: {typeof(TEvent).Name}");
            }

            EventPriority eventPriority = (EventPriority)eventType.GetValue(_remoteConfiguration);

            SimpleLogger.Debug($"Event {eventType} has event priority {eventPriority}");
            return eventPriority;
        }

        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(RemoteConfiguration)} was called before AgentConfiguration initialization");
            }
        }

        private static void OnAgentConfigurationChanged(RemoteConfiguration remoteConfiguration)
        {
            AgentConfigurationChanged?.Invoke(remoteConfiguration);
        }
    }
}