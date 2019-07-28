// <copyright file="ExternalInterfaceFacade.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Configuration;
using System;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.Core.MessageWorker.Clients
{
    /// <summary>
    /// Represents the external interface facade singleton
    /// </summary>
    public class ExternalInterfaceFacade
    {
        /// <summary>
        /// A static instance of the facade
        /// </summary>
        private static Lazy<IExternalInterface> _instance = new Lazy<IExternalInterface>(CreateInstance);

        /// <summary>
        /// Creates the facade instance according to the configured type
        /// </summary>
        private static IExternalInterface CreateInstance()
        {
            try
            {
                IExternalInterface facade = (IExternalInterface) Activator.CreateInstance(LocalConfiguration.ExternalInterface.FacadeType);
                return facade;
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static IExternalInterface Instance => _instance.Value;

        /// <summary>
        /// Disposes the singleton instance
        /// </summary>
        public static void DisposeInstance()
        {
            if (_instance.IsValueCreated)
            {
                _instance.Value.Dispose();
                _instance = new Lazy<IExternalInterface>(CreateInstance); // Hack. Required by UTs at the moment
            }
        }
    }
}
