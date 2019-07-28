// <copyright file="FakeTwinHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Tests.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Contracts.Events;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.Helpers
{
    /// <summary>
    /// Fake twin helper class
    /// This helper, helps coordinates twin configuration changes
    /// Changes made to the twin will be visible to the clientMock immediately, the agent will see the changes
    /// once he reads the twin from the clientMock.
    /// The user can explicitly make the agent to read configuration by using the UpdateAgentTwin method
    /// </summary>
    public class FakeTwinHelper
    {
        //Note: The name should remain with upper case in order for the test to be run on Linux
        private const string DEFAULT_TWIN_PATH = @"DefaultTwin.json";
        private readonly Twin _twin;
        private readonly JToken _agentConfiguration;
        private readonly ModuleClientMock _clientMock;


        /// <summary>
        /// C'tor creates a fakeTwinHelper
        /// The constructor sets the clientMock with an empty twin
        /// </summary>
        /// <param name="clientMock">A ModuleClient mock</param>
        public FakeTwinHelper(ModuleClientMock clientMock) : this(clientMock, DEFAULT_TWIN_PATH)
        {
        }

        /// <summary>
        /// C'tor creates a fakeTwinHelper
        /// The constructor parses the twin from the file path
        /// and sets the clientMock with this twin
        /// </summary>
        /// <param name="clientMock">A ModuleClient mock</param>
        /// <param name="path">Path to a serialized twin json file</param>
        public FakeTwinHelper(ModuleClientMock clientMock, string path)
        {
            _twin = GetTwinFromFile(path);
            _agentConfiguration = _twin.Properties.Desired["ms_iotn:urn_azureiot_Security_SecurityAgentConfiguration"];
            _clientMock = clientMock;
            _clientMock.Reset(_twin);

            UpdateAgentTwinConfiguration();
        }

        /// <summary>
        /// Change event priority in the Twin
        /// </summary>
        /// <param name="eventName">Event name to change</param>
        /// <param name="priority">The desired priority</param>
        public FakeTwinHelper ChangeEventPriority(string eventName, EventPriority priority)
        {
            string propertyName = $"eventPriority{eventName}";
            JObject existingConfiguration = _agentConfiguration.SelectToken(propertyName) as JObject;
            JValue newValue = new JValue(priority.ToString());
            if (existingConfiguration != null)
            {
                existingConfiguration["value"].Replace(newValue);
            }
            else
            {
                JObject priorityObject = new JObject { new JProperty("value", newValue) };
                ((JObject)_agentConfiguration).Add(propertyName, priorityObject);
            }

            _clientMock.SetTwin(_twin);
            _clientMock.RaiseDesiredPropertyUpdateCallback(_twin.Properties.Desired, null);

            return this;
        }

        /// <summary>
        /// Change the value of the given configuration in the twin
        /// </summary>
        /// <param name="configurationName">TwinConfiguration name</param>
        /// <param name="value">The desired value</param>
        public FakeTwinHelper ChangeConfiguration(string configurationName, string value)
        {
            JObject configuration = new JObject
            {
                new JProperty("value", value)
            };
            _agentConfiguration[configurationName] = configuration;
            _clientMock.SetTwin(_twin);
            _clientMock.RaiseDesiredPropertyUpdateCallback(_twin.Properties.Desired, null);

            return this;
        }

        /// <summary>
        /// Retrieves current configuration
        /// </summary>
        /// <param name="configurationName">A TwinConfiguration name</param>
        /// <returns></returns>
        public string GetConfiguration(string configurationName)
        {
            return _agentConfiguration[configurationName]["value"].ToString();
        }

        /// <summary>
        /// Updates the device twin configuration explicitly
        /// This method ensures the device gets the configuration immediately
        /// </summary>
        public void UpdateAgentTwinConfiguration()
        {
            AgentConfiguration.Init();
        }

        /// <summary>
        /// Creates a twin from the given file
        /// </summary>
        /// <param name="path">Path to a serialized twin json file</param>
        /// <returns>Twin</returns>
        public static Twin GetTwinFromFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                var desired = new TwinCollection(reader.ReadToEnd());

                return new Twin
                {
                    Properties = new TwinProperties
                    {
                        Desired = desired
                    }
                };
            }
        }
    }
}