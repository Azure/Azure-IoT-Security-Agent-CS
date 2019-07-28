// <copyright file="AuthenticationTestFromDevice.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests.Authentication
{
    /// <summary>
    /// Test authentication method provider from Device
    /// </summary>
    [TestClass]
    public class AuthenticationTestFromDevice : AuthenticationTestBase
    {
        /// <summary>
        /// Verify that Gateway cannot be empty
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyGatewayConfiguration()
        {
            NameValueCollection invalidConfigCollection = new NameValueCollection()
            {
                { AuthenticationData.TypeKey, AuthenticationMethodProvider.AuthenticationType.SymmetricKey.ToString() },
                { AuthenticationData.IdentityKey, AuthenticationMethodProvider.AuthenticationIdentity.Device.ToString() },
                { AuthenticationData.CertificateLocationKindKey, AuthenticationMethodProvider.CertificateLocation.LocalFile.ToString() },
                { AuthenticationData.FilePathKey, "\var\tmp" },
                { AuthenticationData.DeviceIdKey, "testDevice" },
                { AuthenticationData.GatewayHostNameKey, "" }
            };
            UpdateAuthenticationData(new AuthenticationData(invalidConfigCollection));
            InitAuthenticationMethodProvider();
        }
        /// <summary>
        /// Verify device file cannot be empty in case of certificate
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyDeviceConfiguration()
        {
            NameValueCollection invalidConfigCollection = new NameValueCollection()
            {
                { AuthenticationData.TypeKey, AuthenticationMethodProvider.AuthenticationType.SelfSignedCertificate.ToString() },
                { AuthenticationData.IdentityKey, AuthenticationMethodProvider.AuthenticationIdentity.Device.ToString() },
                { AuthenticationData.CertificateLocationKindKey, AuthenticationMethodProvider.CertificateLocation.LocalFile.ToString() },
                { AuthenticationData.FilePathKey, "" },
                { AuthenticationData.DeviceIdKey, "testDevice" },
                { AuthenticationData.GatewayHostNameKey, "testGateway" }
            };
            UpdateAuthenticationData(new AuthenticationData(invalidConfigCollection));
            InitAuthenticationMethodProvider();
        }
    }
}
