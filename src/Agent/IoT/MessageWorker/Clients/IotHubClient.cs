// <copyright file="IotHubMessagingClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients;
using Microsoft.Azure.IoT.Agent.IoT.Configuration;
using Microsoft.Azure.IoT.Agent.IoT.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients
{
    /// <summary>
    /// The IoT Hub client that enables sending and receiving data from the hub
    /// </summary>
    public class IotHubMessagingClient : IMessagingClient, IDisposable
    {
        /// <summary>
        /// Cancellation token for canceling IoTHubClient scheduled jobs
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// A flag indicating if this object was already disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// A singleton instance  of the client that is used to connect the IoT Hub
        /// </summary>
        private readonly IModuleClient _moduleClient = ModuleClientProvider.GetClient();

        /// <summary>
        /// A singleton instance of the <see cref="IotHubMessagingClient"/>
        /// </summary>
        private static Lazy<IotHubMessagingClient> _instance = new Lazy<IotHubMessagingClient>(() => new IotHubMessagingClient());

        /// <summary>
        /// Gets the singleton instance of <see cref="IotHubMessagingClient"/>
        /// </summary>
        public static IotHubMessagingClient Instance => _instance.Value;

        /// <summary>
        /// Static ctor
        /// </summary>
        private IotHubMessagingClient()
        {
            if (_moduleClient == null)
            {
                SimpleLogger.Error("Failed to create device client");

                // TODO: make it a specific exception
                throw new Exception("Failed to create device client");
            }

            //Register to be called when twin is changed.
            //Note the link below, there is a bug that requires OpenAsync to be called first
            //See here for more details: https://github.com/Azure/azure-iot-sdk-csharp/issues/45
            _moduleClient.OpenAsync();
        }     

        /// <summary>
        /// Disposing the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the singleton instance
        /// </summary>
        public static void DisposeInstance()
        {
            if (_instance.IsValueCreated)
            {
                _instance.Value.Dispose();
                _instance = new Lazy<IotHubMessagingClient>(() => new IotHubMessagingClient());
            }
        }


        /// <summary>
        /// Retrieve a device twin object for the current client
        /// </summary>
        /// <returns>The device twin object</returns>
        public Task<Twin> GetTwinAsync()
        {
            return _moduleClient.GetTwinAsync();
        }

        /// <summary>
        /// Send a message to the hub
        /// </summary>
        /// <param name="messageBody">The message to send</param>
        /// <param name="messageProperties">Additional message properties</param>
        /// <returns>Task that can be awaited for the send completion</returns>
        public async Task SendMessage(AgentMessage messageBody, Dictionary<string, string> messageProperties)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            RemoteIoTConfiguration remoteIoTConfiguration = (RemoteIoTConfiguration)AgentConfiguration.RemoteConfiguration;
            messageProperties.Add("HubResourceId", remoteIoTConfiguration.HubResourceId);

            string messageBuffer = messageBody.GetMessageBodyBuffer();
            Message iotHubMessage = new Message(Encoding.UTF8.GetBytes(messageBuffer));

            foreach (var property in messageProperties)
            {
                iotHubMessage.Properties.Add(property);
            }

            iotHubMessage.SetAsSecurityMessage();

            await _moduleClient.SendEventAsync(iotHubMessage);
        }

        /// <summary>
        /// Disposing the object
        /// </summary>
        /// <param name="disposing">Flag indicating dispose state</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                // The module client initialized in the ctor cannot be null
                _cancellationTokenSource.Cancel();
                _moduleClient.Dispose();
                _cancellationTokenSource.Dispose();
                _isDisposed = true;
            }
        }
    }
}