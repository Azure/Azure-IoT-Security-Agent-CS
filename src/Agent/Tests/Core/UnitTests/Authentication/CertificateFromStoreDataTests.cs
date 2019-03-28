// <copyright file="CertificateFromStoreDataTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Tests.Common.UnitTests.Authentication
{
    /// <summary>
    /// Tests for <see cref="CertificateFromStoreData"/>
    /// </summary>
    [TestClass]
    public class CertificateFromStoreDataTests
    {
        /// <summary>
        /// Deserialization tests
        /// </summary>
        [TestClass]
        public class Deserialization : CertificateFromStoreDataTests
        {
            /// <summary>
            /// StoreLocation should be mandatory
            /// </summary>
            [TestMethod]
            public void ShouldThrowIfStoreLocationIsMissing()
            {
                const string json = "{\"storeName\":\"name\",\"certificateThumbprint\":\"thumbprint\"}";
                Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<CertificateFromStoreData>(json));
            }

            /// <summary>
            /// StoreName should be mandatory
            /// </summary>
            [TestMethod]
            public void ShouldThrowIfStoreNameIsMissing()
            {
                const string json = "{\"storeLocation\":\"location\",\"certificateThumbprint\":\"thumbprint\"}";
                Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<CertificateFromStoreData>(json));
            }

            /// <summary>
            /// Certificate thumbprint should be mandatory
            /// </summary>
            [TestMethod]
            public void ShouldThrowIfCertificateThumbprintIsMissing()
            {
                const string json = "{\"storeName\":\"name\",\"storeLocation\":\"location\"}";
                Assert.ThrowsException<JsonSerializationException>(() => JsonConvert.DeserializeObject<CertificateFromStoreData>(json));
            }
        }
    }
}