// <copyright file="IModuleClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients
{
    /// <summary>
    /// ModuleCLient interface
    /// </summary>
    public interface IModuleClient : IDisposable
    {
        /// <summary>
        /// Send the message to the IoTHub
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <returns>Awaitable task</returns>
        Task SendEventAsync(Message message);

        /// <summary>
        /// Gets the device's remote twin
        /// </summary>
        /// <returns>Awaitable task with device twin</returns>
        Task<Twin> GetTwinAsync();

        /// <summary>
        /// Explicitly open the DeviceClient instance.
        /// </summary>
        /// <returns>Task </returns>
        Task OpenAsync();

        /// <summary>
        /// Set a callback that will be called whenever the client receives a twin state update (desired or reported) from the service
        /// </summary>
        /// <param name="callback">Callback to call after the state update has been received and applied</param>
        /// <param name="userContext">Context object that will be passed into callback</param>
        /// <returns>Task</returns>
        Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext);

        /// <summary>
        /// Updates reported config in module twin
        /// </summary>
        /// <param name="reported">Reported properties to update</param>
        /// <returns>Awaitable task</returns>
        Task UpdateRportedPropertiesAsync(TwinCollection reported);

        /// <summary>
        /// Set a callback that will be called whenever the connection status changes
        /// </summary>
        /// <param name="statusChangesHandler">the callback</param>
        void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler);

        /// <summary>
        /// Re-Inits the Module Client with the given connection string and transport type
        /// </summary>
        /// <param name="connectionString">the connection string</param>
        /// <param name="transportType">the transport type</param>
        void ReInit(string connectionString, TransportType transportType);

    }
}