// <copyright file="CertificateLoader.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// Responsible for loading certificates
    /// </summary>
    public class CertificateLoader
    {
        /// <summary>
        /// Loads a certificate
        /// </summary>
        /// <param name="certificateLocation">The kind of location that the  certificate will be loaded from</param>
        /// <param name="certificateInfoFilePath">Path to the file that will be used to load the certificate. The expected content depends on the value of 
        /// <see cref="AuthenticationMethodProvider.CertificateLocation"/></param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="MisconfigurationException" />
        /// <returns>The loaded certificate</returns>
        public static X509Certificate2 Load(AuthenticationMethodProvider.CertificateLocation certificateLocation, string certificateInfoFilePath)
        {
            if (certificateInfoFilePath == null)
                throw new ArgumentNullException(nameof(certificateInfoFilePath));

            switch (certificateLocation)
            {
                case AuthenticationMethodProvider.CertificateLocation.LocalFile:
                    {
                        X509Certificate2 certificate = new X509Certificate2(certificateInfoFilePath);
                        return certificate;
                    }
                case AuthenticationMethodProvider.CertificateLocation.Store:
                    {
                        string certificateInfoJson = File.ReadAllText(certificateInfoFilePath);
                        CertificateFromStoreData certificateStoreInfo = JsonConvert.DeserializeObject<CertificateFromStoreData>(certificateInfoJson);
                  
                        using (var store = new X509Store(certificateStoreInfo.StoreName, certificateStoreInfo.StoreLocation))
                        {
                            store.Open(OpenFlags.ReadOnly);

                            X509Certificate2Collection certficateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, certificateStoreInfo.CertificateThumbprint, false);
                            if (certficateCollection.Count != 1)
                            {
                                throw new MisconfigurationException(
                                    $"Certificate wasn't found in store (StoreName: {store.Name}, StoreLocation: {store.Location}, CertificateThumbprint: {certificateStoreInfo.CertificateThumbprint})");
                            }

                            X509Certificate2 certificate = certficateCollection[0];
                            return certificate;
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(certificateLocation), certificateLocation, "Value not supported");
            }
        }
    }
}
