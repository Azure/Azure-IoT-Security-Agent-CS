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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.IoT.Agent.IoT.Exceptions;
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
        public readonly string ConfigSectionName;

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
            ConfigSectionName = LocalIoTHubConfiguration.IotInterface.RemoteConfigurationObject;
        }

        /// <summary>
        /// Event handler for DesiredProperty updates from the remote twin
        /// Raises the <see cref="RemoteConfigurationChanged"/> event
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
            JObject readyToDeserializeTwinConfiguration = GetAgentConfigurationObjectFromTwin(desiredProperties);
            TRemoteConfiguration twinConfiguration = DeserializeTwin(readyToDeserializeTwinConfiguration, out List<string> errors);

            if (errors.Any())
            {
                OnFailedToParseConfiguration(errors);
                throw new AgentException(ExceptionCodes.RemoteConfiguration, ExceptionSubCodes.CantParseConfiguration, $"Can't parse the following properties: {string.Join(", ", errors)}");
            }

            return twinConfiguration;
        }

        /// <summary>
        /// Reads and prepares the agentConfiguration section for deserialization 
        /// </summary>
        private JObject GetAgentConfigurationObjectFromTwin(TwinCollection desiredProperties)
        {
            if (desiredProperties.Contains(ConfigSectionName))
            {
                JObject configSection = desiredProperties[ConfigSectionName];
                if (configSection != null)
                {
                    //remove null values from the json
                    return HandleNullValues(configSection);
                }
            }

            return new JObject();
        }

        /// <summary>
        /// Connect to the Hub to get the twin data
        /// </summary>
        /// <returns>TwinCollection of the desired properties</returns>
        /// <exception cref="TimeoutException">Throw exception if couldn't get the twin after configurable timeout</exception>
        private TwinCollection GetTwinCollection()
        {
            var twinTask = IotHubMessagingClient.Instance.GetTwinAsync();
            bool isConfigurationRead;
            try
            {
                isConfigurationRead = twinTask.Wait(LocalConfiguration.General.ReadRemoteConfigurationTimeout);
            }
            catch (AggregateException ex) when (ex.InnerException is DeviceNotFoundException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.NotFound, "Validate authentication configuration");
            }
            catch (AggregateException ex) when (ex.InnerException is UnauthorizedException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Unauthorized, "Validate authentication configuration");
            }
            if (!isConfigurationRead)
            {
                throw new AgentException(ExceptionCodes.RemoteConfiguration, ExceptionSubCodes.Timeout, "Failed to read security module twin in time");
            }

            Twin twin = twinTask.Result;
            TwinCollection collection = twin.Properties.Desired;
            return collection;
        }

        /// <summary>
        /// Remove keys with null values
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        private JObject HandleNullValues(JObject jObject)
        {
            JObject noNullConfigSection = JsonUtils.RemoveKeysWithNullValue(jObject);
            return noNullConfigSection;
        }

        /// <summary>
        /// Deserialize agent twin configuration,
        /// </summary>
        /// <param name="readyToDeserializeTwinConfiguration">ready to deserialize twin json</param>
        /// <param name="outError">List of properties that could not be parsed</param>
        /// <returns>TwinConfiguration</returns>
        private TRemoteConfiguration DeserializeTwin(JToken readyToDeserializeTwinConfiguration, out List<string> outError)
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

            outError = errorList;
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
