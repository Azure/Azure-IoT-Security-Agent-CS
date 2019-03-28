// <copyright file="ModuleClientMock.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.IoT.Agent.IoT.MessageWorker.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.Client
{
    /// <summary>
    /// ModuleClientMock
    /// A local client, 
    /// this class implements IModuleClient allowing the user to run the agent w/o having a connection to an IoThub
    /// </summary>
    public class ModuleClientMock : IModuleClient
    {
        private DesiredPropertyUpdateCallback _desiredPropertyUpdateCallback;
        private readonly List<Message> _sentMessages = new List<Message>();
        private Twin _fakeTwin;

        /// <summary>
        /// Creates a new LocalMoudleClient with an empty twin
        /// </summary>
        public ModuleClientMock() : this(new Twin())
        {
        }

        /// <summary>
        /// Create a new ModuleClientMock with the given Twin
        /// </summary>
        /// <param name="twin">Twin to set on the client</param>
        public ModuleClientMock(Twin twin)
        {
            Reset(twin);
        }

        /// <summary>
        /// Set the client with a new device twin
        /// </summary>
        /// <param name="twin">Twin to set on the client</param>
        public void SetTwin(Twin twin)
        {
            _fakeTwin = CopyTwin(twin);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<Twin> GetTwinAsync()
        {
            return Task.FromResult(CopyTwin(_fakeTwin));
        }

        /// <summary>
        /// In tests this function is not supported
        /// </summary>
        /// <returns>Task.CompletedTask</returns>
        public Task OpenAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// In tests we don't support actual callback for twin changes
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="userContext"></param>
        /// <returns>Task.CompletedTask</returns>
        public Task SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdateCallback callback, object userContext)
        {
            _desiredPropertyUpdateCallback = callback;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task UpdateRportedPropertiesAsync(TwinCollection reported)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Raises the <see cref="DesiredPropertyUpdateCallback"/> event
        /// </summary>
        public void RaiseDesiredPropertyUpdateCallback(TwinCollection desiredProperties, object userContext)
        {
            _desiredPropertyUpdateCallback?.Invoke(desiredProperties, userContext);
        }
 
        /// <summary>
        /// <inheritdoc />
        /// Sends the message locally. The messages are stored internally in the client
        /// </summary>
        public Task SendEventAsync(Message iotHubMessage)
        {
            _sentMessages.Add(iotHubMessage);

            return Task.Delay(TimeSpan.FromMilliseconds(10));
        }

        /// <summary>
        /// Get all sent messages
        /// </summary>
        /// <returns>List of sent messages</returns>
        public List<Message> GetMessages()
        {
            return _sentMessages.ToList();
        }

        /// <summary>
        /// Resets the client to initial state with an empty twin
        /// </summary>
        public void Reset()
        {
            Reset(new Twin());
        }

        /// <summary>
        /// Resets the client to initial state with the given twin
        /// </summary>
        /// <param name="twin">Twin to set on the client
        /// </param>
        public void Reset(Twin twin)
        {
            _sentMessages.Clear();
            _fakeTwin = CopyTwin(twin);
        }

        /// <summary>
        /// Creates a new twin with the given twin properties
        /// </summary>
        /// <param name="twinToCopy">Twin to copy</param>
        /// <returns>New twin with same properties as twin to copy</returns>
        private Twin CopyTwin(Twin twinToCopy)
        {
            TwinCollection desired = new TwinCollection(twinToCopy.Properties.Desired.ToJson());
            TwinCollection reported = new TwinCollection(twinToCopy.Properties.Reported.ToJson());

            return new Twin(new TwinProperties()
            {
                Desired = desired,
                Reported = reported
            });
        }
    }
}