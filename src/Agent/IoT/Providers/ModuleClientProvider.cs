// <copyright file="ModuleClientProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.IoT.Providers
{
    /// <summary>
    /// Module client provider
    /// The provider provides an IModuleClient
    /// The user can set its own client by adding ModuleClientDll and keys in App.Config
    /// if the user does not set the keys above the provider will provide the default Azure IoT sdk's ModuleClient with the
    /// connection string
    /// from App.Config
    /// </summary>
    public class ModuleClientProvider
    {
        /// <summary>
        /// IModuleClient client
        /// </summary>
        private static IModuleClient _client;

        /// <summary>
        /// Gets the client
        /// </summary>
        public static IModuleClient GetClient()
        {
            return _client ?? (_client = CreateClient());
        }

        /// <summary>
        /// If a ModuleClient configuration is specified,
        /// creates a module client from the configuration,
        /// Otherwise creates a ModuleClientWrapper which delegates to the IoT sdk's ModuleClient.
        /// according to the LocalConfiguration.ClientConnectionString
        /// </summary>
        /// <returns>A new instance of IModuleClient</returns>
        private static IModuleClient CreateClient()
        {
            NameValueCollection agentConfig = (NameValueCollection)ConfigurationManager.GetSection("General");
            var moduleClientDll = agentConfig["ModuleClientDll"];
            var moduleClientTypeName = agentConfig["ModuleClientFullName"];

            if (!string.IsNullOrEmpty(moduleClientDll) && !string.IsNullOrEmpty(moduleClientTypeName))
            {
                Assembly assembly = Assembly.LoadFrom(Path.GetFullPath(moduleClientDll));
                Type type = assembly.GetType(moduleClientTypeName);

                return (IModuleClient) Activator.CreateInstance(type);
            }
         
            return new ModuleClientWrapper(ModuleClient.CreateFromConnectionString(
                AuthenticationMethodProvider.GetModuleConnectionString(),
                LocalIoTHubConfiguration.IotInterface.TransportType));
        }
    }
}  