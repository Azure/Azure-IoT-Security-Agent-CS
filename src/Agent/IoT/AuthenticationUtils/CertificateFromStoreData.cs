// <copyright file="CertificateFromStoreData.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// Contains data required to load a certificate from a certificate store
    /// </summary>
    public class CertificateFromStoreData
    {
        /// <summary>
        /// The location of the certificate store
        /// </summary>
        [JsonProperty(PropertyName =  "storeLocation",  Required = Required.Always)]
        public StoreLocation StoreLocation { get; set; }

        /// <summary>
        /// The name of the certificate store
        /// </summary>
        [JsonProperty(PropertyName = "storeName", Required = Required.Always)]
        public StoreName StoreName { get; set; }

        /// <summary>
        /// The certificate's thumbprint
        /// </summary>
        [JsonProperty(PropertyName = "certificateThumbprint", Required = Required.Always)]
        public string CertificateThumbprint { get; set; }
    }
}
