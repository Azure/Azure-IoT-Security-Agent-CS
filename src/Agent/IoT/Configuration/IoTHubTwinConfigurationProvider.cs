// <copyright file="IoTHubTwinConfigurationProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients;
using Microsoft.Azure.IoT.Agent.IoT.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.IoT.Agent.IoT.Configuration
{
    /// <summary>
    /// Provides <see cref="RemoteConfiguration"/> updates from the IoTHub
    /// </summary>
    public class IoTHubTwinConfigurationProvider<TRemoteConfiguration> : IRemoteConfigurationProvider where TRemoteConfiguration : RemoteIoTConfiguration
    {
        /// <summary>
        /// Event handlers for FailedToParseConfiguration
        /// </summary>
        public event FailedToParseConfiguration FailedToParseConfiguration;

        /// <summary>
        /// Event that notifies about configuration updates
        /// </summary>
        public event RemoteConfigurationChangedEventHandler RemoteConfigurationChanged;

        /// <summary>
        /// The name of the configuration section in the twin
        /// </summary>
        public const string ConfigSectionName = "azureiot*com^securityAgentConfiguration^1*0*0";

        /// <summary>
        /// The iot hub module client
        /// </summary>
        private readonly IModuleClient _moduleClient;

        /// <summary>
        /// Ctor
        /// </summary>
        public IoTHubTwinConfigurationProvider()
        {
            AgentConfiguration.AgentConfigurationChanged += UpdateReportedProperties;

            _moduleClient = ModuleClientProvider.GetClient();
            _moduleClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyUpdateCallback, null);
        }

        /// <summary>
        /// Event handler for DesiredProperty updates from the remote twin
        /// Updates current local <see cref="TRemoteConfiguration"/>
        /// </summary>
        private Task OnDesiredPropertyUpdateCallback(TwinCollection desiredProperties, object userContext)
        {
            TRemoteConfiguration twinConfiguration;

            try
            {
                twinConfiguration = ReadTwinConfiguration(desiredProperties);
            }
            catch (Exception ex)
            {
                SimpleLogger.Error("Twin syntax error. configuration was not changed", exception: ex);  
                return Task.CompletedTask;
            }

            RemoteConfigurationChanged?.Invoke(twinConfiguration);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets an updated <see cref="RemoteConfiguration"/> instance
        /// </summary>
        public RemoteConfiguration GetRemoteConfigurationData()
        {
            TwinCollection twinCollection = GetTwinCollection();
            TRemoteConfiguration twinConfiguration = ReadTwinConfiguration(twinCollection);
    
            return twinConfiguration;
        }

        /// <summary>
        /// Reads the agentConfiguration section from the given <see cref="TwinCollection"/>
        /// </summary>
        private TRemoteConfiguration ReadTwinConfiguration(TwinCollection desiredProperties)
        {
            try
            {
                JToken readyToDeserializeTwinConfiguration = PrepareTwinForDeserialization(desiredProperties);
                TRemoteConfiguration twinConfiguration = DeserializeTwin(readyToDeserializeTwinConfiguration);
                return twinConfiguration;
            }
            catch (RemoteConfigurationParsingException ex)
            {
                OnFailedToParseConfiguration(ex.MisconfiguredProperties);
                throw;
            }
        }

        /// <summary>
        /// Reads and prepares the agentConfiguration section for deserialization 
        /// </summary>
        private JToken PrepareTwinForDeserialization(TwinCollection twinConfCollection)
        {
            JObject configSection = GetConfigurationSection(twinConfCollection);

            if (configSection == null)
            {
                throw new RemoteConfigurationParsingException(new List<string>() { ConfigSectionName });
            }

            //remove null values from the json
            return HandleNullValues(configSection);
        }

        /// <summary>
        /// Connect to the Hub to get the twin data
        /// </summary>
        /// <returns>TwinCollection of the desired properties</returns>
        /// <exception cref="TimeoutException">Throw exception if couldn't get the twin after configurable timeout</exception>
        private TwinCollection GetTwinCollection()
        {
            var twinTask = IotHubMessagingClient.Instance.GetTwinAsync();
            if (!twinTask.Wait(LocalConfiguration.General.ReadRemoteConfigurationTimeout))
            {
                throw new TimeoutException("Failed to read security module twin in time");
            }

            Twin twin = twinTask.Result;
            TwinCollection collection = twin.Properties.Desired;
            return collection;
        }

        /// <summary>
        /// Get the configuration section from the twin
        /// </summary>
        /// <param name="twinCollection">The twin collection</param>
        /// <returns>A JToken representing the configuration section</returns>
        /// <exception cref="MissingFieldException">In case the twin doesn't contain agentConfiguration section</exception>
        private JObject GetConfigurationSection(TwinCollection twinCollection)
        {
            foreach (KeyValuePair<string, Object> child in twinCollection)
            {
                if (child.Key == ConfigSectionName)
                    return child.Value as JObject;
            }

            throw new MisconfigurationException($"The twin doesn't contain {ConfigSectionName} section");
        }

        /// <summary>
        /// Remove keys with null values
        /// </summary>
        /// <param name="jtoken"></param>
        /// <returns></returns>
        private JToken HandleNullValues(JToken jtoken)
        {
            JToken noNullConfigSection = JsonUtils.RemoveKeysWithNullValue(jtoken);
            return noNullConfigSection;
        }

        /// <summary>
        /// Deserialize agent twin configuration,
        /// throws if can't deserialize
        /// </summary>
        /// <param name="readyToDeserializeTwinConfiguration">ready to deserialize twin json</param>
        /// <returns>TwinConfiguration</returns>
        private TRemoteConfiguration DeserializeTwin(JToken readyToDeserializeTwinConfiguration)
        {
            var serializer = new JsonSerializer();
            var errorList = new List<string>();
            serializer.Error += (sender, args) =>
            {
                errorList.Add(args.ErrorContext.Path);
                args.ErrorContext.Handled = true;
            };

            serializer.Converters.Add(new IsoTimespanConverter());

            var deserializedTwin = readyToDeserializeTwinConfiguration.ToObject<TRemoteConfiguration>(serializer);

            if (errorList.Count != 0)
            {
                throw new RemoteConfigurationParsingException(errorList);
            }

            return deserializedTwin;
        }

        /// <summary>
        /// Handler function for failing configuration parsing
        /// </summary>
        /// <param name="configurations">The misconfigured properties</param>
        private void OnFailedToParseConfiguration(IList<string> configurations)
        {
            FailedToParseConfiguration?.Invoke(configurations);
        }

        /// <summary>
        /// Update module twin reported properites with current agent configuration 
        /// </summary>
        private void UpdateReportedProperties(RemoteConfiguration remoteConfiguration)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new StringEnumConverter());
            serializer.Converters.Add(new IsoTimespanConverter());

            var reportedConfiguration = new JObject();
            reportedConfiguration[ConfigSectionName] = JObject.FromObject(remoteConfiguration, serializer);

            var reportedCollection = new TwinCollection(reportedConfiguration, new JObject());

            _moduleClient.UpdateRportedPropertiesAsync(reportedCollection);
        }
    }
}
