// <copyright file="ExternalInterfaceConfig.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.Core.Configuration.ConfigurationSectionHandlers
{
    public class ExternalInterfaceConfig
    {
        /// <summary>
        /// The fixed name of the ExternalInterfaceFacadeType key
        /// </summary>
        public const string FacadeTypeKey = "facadeType";

        /// <summary>
        /// Gets the type to be used as the facade for the external interface
        /// </summary>
        public Type FacadeType { get; }

        public ExternalInterfaceConfig(NameValueCollection nameValueCollection)
        {
            string facadeTypeName = nameValueCollection[FacadeTypeKey];
            FacadeType = Type.GetType(facadeTypeName, true);
        }
    }
}
