// <copyright file="IoTInterfaceConfig.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers;
using System;
using System.Collections.Specialized;

namespace Microsoft.Azure.IoT.Agent.IoT.Configuration
{
    public class IoTInterfaceConfig : ExternalInterfaceConfig
    {
        /// <summary>
        /// The fixed name of the TransportType key
        /// </summary>
        public const string TransportTypeKey = "transportType";

        /// <summary>
        /// Gets the transport type to use
        /// </summary>
        public TransportType TransportType { get; private set; }

        public IoTInterfaceConfig(NameValueCollection nameValueCollection) : base(nameValueCollection)
        {
            TransportType = (TransportType)Enum.Parse(typeof(TransportType), nameValueCollection[TransportTypeKey]);
        }
    }
}
