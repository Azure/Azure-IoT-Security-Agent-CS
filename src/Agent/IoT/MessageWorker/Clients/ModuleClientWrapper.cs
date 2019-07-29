// <copyright file="ModuleClientWrapper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients
{
    /// <summary>
    /// Delegate to Azure IoT Sdk ModuleClient
    /// The wrapper is required for the sdk's ModuleClient to implement the IModuleClient interface
    /// </summary>
    public class ModuleClientWrapper : IModuleClient
    {
        private ModuleClient _client;

        /// <summary>
        /// C-tor creates a new wrapper object with the given client
        /// </summary>
        /// <param name="client">Azure IoT Sdk ModuleClient</param>
        public ModuleClientWrapper(ModuleClient client)
        {
            _client = client;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _client.Dispose();
        }

        /// <inheritdoc />
        public Task<Twin> GetTwinAsync()
        {
            return _client.GetTwinAsync();
        }

        /// <inheritdoc />
        public Task SendEventAsync(Message message)
        {
            return _client.SendEventAsync(message);
        }

        /// <inheritdoc />
        public Task OpenAsync()
        {
            return _client.OpenAsync();
        }

        /// <inheritdoc />
        public Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            return _client.SetDesiredPropertyUpdateCallbackAsync(callback, userContext);
        }

        /// <inheritdoc />
        public async Task UpdateRportedPropertiesAsync(TwinCollection reported)
        {
            await _client.UpdateReportedPropertiesAsync(reported);
        }

        /// <inheritdoc />
        public void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler)
        {
            _client.SetConnectionStatusChangesHandler(statusChangesHandler);
        }

        /// <inheritdoc />
        public void ReInit(string connectionString, TransportType transportType)
        {
            var newClient = ModuleClient.CreateFromConnectionString(connectionString, transportType);
            _client.Dispose();
            _client = newClient;
        }
    }
}