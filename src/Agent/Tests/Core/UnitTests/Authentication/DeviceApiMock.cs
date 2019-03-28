// <copyright file="DeviceApiMock.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.IoT.RestApis;
using Newtonsoft.Json.Linq;

namespace Agent.Tests.Common.UnitTests.Authentication
{
    /// <summary>
    /// Mock for IDeviceApi
    /// Bypass the actual sending rest request to the IoT hub
    /// </summary>
    public class DeviceApiMock : IDeviceApi
    {
        /// <inheritdoc />
        public JToken GetDeviceAgentModule(string deviceId)
        {
            string module =
                "{\r\n  \"moduleId\": \"azureiotsecurity\",\r\n  \"managedBy\": null,\r\n  \"deviceId\": \"sastoken\",\r\n  \"generationId\": \"636784789899007228\",\r\n  \"etag\": \"Mzk0MDUwODg0\",\r\n  \"connectionState\": \"Disconnected\",\r\n  \"connectionStateUpdatedTime\": \"2018-11-25T09:08:48.4552429\",\r\n  \"lastActivityTime\": \"0001-01-01T00:00:00\",\r\n  \"cloudToDeviceMessageCount\": 0,\r\n  \"authentication\": {\r\n    \"symmetricKey\": {\r\n      \"primaryKey\": \"myTestPrimaryKey\",\r\n      \"secondaryKey\": \"myTestSecondaryKey\"\r\n    },\r\n    \"x509Thumbprint\": {\r\n      \"primaryThumbprint\": null,\r\n      \"secondaryThumbprint\": null\r\n    },\r\n    \"type\": \"sas\"\r\n  }\r\n}";
            JToken output = JToken.Parse(module);
            return output;
        }
    }
}
