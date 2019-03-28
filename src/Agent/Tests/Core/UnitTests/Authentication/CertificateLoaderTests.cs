// <copyright file="CertificateLoaderTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Tests.Common.UnitTests.Authentication
{
    /// <summary>
    /// Tests for the <see cref="CertificateLoader"/> class
    /// </summary>
    [TestClass]
    public class CertificateLoaderTests
    {
        private static readonly string _tempCertificatePath = Path.GetTempFileName();
        private static readonly string _tempStoreInfoPath = Path.GetTempFileName();
        private static X509Certificate2 _expectedCertificate;
        private static X509Store _expectedCertificateStore;

        /// <summary>
        /// Initialize the testing environment
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _expectedCertificate = new X509Certificate2(CertificateTestData.MsCertificate);

            var certificateFromStoreInfo = new CertificateFromStoreData()
            {
                CertificateThumbprint = _expectedCertificate.Thumbprint,
                StoreLocation = StoreLocation.CurrentUser,
                StoreName = StoreName.My
            };

            _expectedCertificateStore = new X509Store(certificateFromStoreInfo.StoreName, certificateFromStoreInfo.StoreLocation);
            _expectedCertificateStore.Open(OpenFlags.ReadWrite);
            _expectedCertificateStore.Add(_expectedCertificate);
            _expectedCertificateStore.Close();

            File.WriteAllBytes(_tempCertificatePath, CertificateTestData.MsCertificate);

            string certificateFromStoreInfoJson = JsonConvert.SerializeObject(certificateFromStoreInfo);
            File.WriteAllText(_tempStoreInfoPath, certificateFromStoreInfoJson);
        }

        /// <summary>
        /// Cleanup the testing environment
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _expectedCertificateStore.Open(OpenFlags.ReadWrite);
            _expectedCertificateStore.Close();
            _expectedCertificateStore.Dispose();

            _expectedCertificate.Dispose();

            File.Delete(_tempCertificatePath);
            File.Delete(_tempStoreInfoPath);
        }

        /// <summary>
        /// Tests for the Load method
        /// </summary>
        [TestClass]
        public class LoadMethod : CertificateLoaderTests
        {
            /// <summary>
            /// <see cref="ClassInitialize"/> is non-inheritable, so make sure it's called on the derived class
            /// </summary>
            /// <param name="context"></param>
            [ClassInitialize]
            public static new void ClassInitialize(TestContext context)
            {
                CertificateLoaderTests.ClassInitialize(context);
            }

            /// <summary>
            /// <see cref="ClassCleanup"/> is non-inheritable, so make sure it's called on the derived class
            /// </summary>
            [ClassCleanup]
            public static new void ClassCleanup()
            {
                CertificateLoaderTests.ClassCleanup();
            }

            /// <summary>
            /// An exception should be thrown if the filePath argument was passed as null
            /// </summary>
            [TestMethod]
            public void ShouldThrowOnFilePathNotSupplied()
            {
                Assert.ThrowsException<ArgumentNullException>(() => { using (var x = CertificateLoader.Load(AuthenticationMethodProvider.CertificateLocation.LocalFile, null)) { } });
            }

            /// <summary>
            /// Test certificate loading for a local file
            /// </summary>
            [TestMethod]
            public void ShouldLoadCertificateFromLocalFile()
            {
                using (X509Certificate2 certificate = CertificateLoader.Load(AuthenticationMethodProvider.CertificateLocation.LocalFile, _tempCertificatePath))
                {
                    Assert.AreEqual(_expectedCertificate.Thumbprint, certificate.Thumbprint);
                }
            }

            /// <summary>
            /// An exception should be thrown if an unsupported <see cref="AuthenticationMethodProvider.CertificateLocation"/> was passed
            /// </summary>
            [TestMethod]
            public void ShouldThrowOnUnexpectedCertificateLocation()
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => { using (var x = CertificateLoader.Load((AuthenticationMethodProvider.CertificateLocation)int.MaxValue, string.Empty)) { } });
            }

            /// <summary>
            /// An exception should be thrown if the requested certificate doesn't exist in the store
            /// </summary>
            [TestMethod]
            public void ShouldThrowOnCertificateNotFoundInStore()
            {
                _expectedCertificateStore.Open(OpenFlags.ReadWrite);
                _expectedCertificateStore.Remove(_expectedCertificate);
                _expectedCertificateStore.Close();

                try
                {
                    Assert.ThrowsException<MisconfigurationException>(() => { using (var x = CertificateLoader.Load(AuthenticationMethodProvider.CertificateLocation.Store, _tempStoreInfoPath)) { } });
                }
                finally
                {
                    _expectedCertificateStore.Open(OpenFlags.ReadWrite);
                    _expectedCertificateStore.Add(_expectedCertificate);
                    _expectedCertificateStore.Close();
                }
            }

            /// <summary>
            /// Test certificate loading from the store
            /// </summary>
            [TestMethod]
            public void ShouldLoadCertificateFromStore()
            {
                X509Certificate2 certificate = CertificateLoader.Load(AuthenticationMethodProvider.CertificateLocation.Store, _tempStoreInfoPath);
                Assert.AreEqual(_expectedCertificate.Thumbprint, certificate.Thumbprint);
            }
        }
    }
}
