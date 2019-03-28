// <copyright file="MessageHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Tests.Client;
using Microsoft.Azure.IoT.Agent.Core.Tests.FakeEvents;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.Helpers
{
    /// <summary>
    /// Helper class for verifying messages and events
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// Extracts and desrializes events from a SecurityMessage
        /// </summary>
        /// <param name="msgBytes">A message body bytes to extract event from</param>
        /// <returns>The events</returns>
        public static IEnumerable<Event> GetEventsFromMsg(byte[] msgBytes)
        {
            var result = Encoding.UTF8.GetString(msgBytes);
            JToken jsonMsg = JObject.Parse(result);

            return JsonConvert.DeserializeObject<IEnumerable<Event>>(jsonMsg["Events"].ToString());
        }

        /// <summary>
        /// Verifies message header according to the params in App.config
        /// </summary>
        /// <param name="msgBytes">A message body bytes to verify</param>
        public static void VerifyMessageHeader(byte[] msgBytes)
        {
            var result = Encoding.UTF8.GetString(msgBytes);
            JToken jsonMsg = JObject.Parse(result);

            NameValueCollection generalConfig = (NameValueCollection)ConfigurationManager.GetSection("General");

            Assert.AreEqual(Assembly.GetAssembly(typeof(AgentBase)).GetName().Version.ToString(), jsonMsg["AgentVersion"]);
            Assert.AreEqual(generalConfig["agentId"], jsonMsg["AgentId"]);
            Assert.AreEqual("1.0", jsonMsg["MessageSchemaVersion"]);
        }

        /// <summary>
        /// Validates that actual event is equal to expected event
        /// </summary>
        /// <param name="actual">Actual value</param>
        /// <param name="expected">Expected value</param>
        public static void ValidateEventsAreEqual(Event actual, EventBase<FakeEventPayload> expected)
        {
            Assert.AreEqual(expected.Category, actual.Category);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.EventType, actual.EventType);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.PayloadSchemaVersion, actual.PayloadSchemaVersion);
            Assert.AreEqual(expected.TimestampLocal.ToString(CultureInfo.InvariantCulture),
                actual.TimestampLocal.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(expected.TimestampUTC.ToString(CultureInfo.InvariantCulture),
                actual.TimestampUTC.ToString(CultureInfo.InvariantCulture));

            var expectedPayloads = expected.Payload.ToArray();
            var actualPayloads = actual.Payload.ToArray();

            Assert.AreEqual(expectedPayloads.Length, actualPayloads.Length);

            for (int i = 0; i < actualPayloads.Length; i++)
            {
                Assert.AreEqual(expectedPayloads[i].Param1, actualPayloads[i].Param1);
                Assert.AreEqual(expectedPayloads[i].Param2, actualPayloads[i].Param2);
            }
        }

        /// <summary>
        /// Heuristically waits until all of the async send-message operations are complete
        /// </summary>
        /// <param name="module">The client module which is used to send the messages</param>
        /// <returns>true if the sends were completed within the alloted time; false if otherwise</returns>
        public static bool WaitUntilAsyncMessageSendsComplete(ModuleClientMock module)
        {
            bool completed = Task.Run(async () =>
            {
                int lastSentMessages = 0;
                while (lastSentMessages < module.GetMessages().Count)
                {
                    lastSentMessages = module.GetMessages().Count;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            })
            .Wait(TimeSpan.FromMinutes(1));

            return completed;
        }
    }
}