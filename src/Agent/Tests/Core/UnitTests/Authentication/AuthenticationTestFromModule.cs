// <copyright file="AuthenticationTestFromModule.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests.Authentication
{
    /// <summary>
    /// Test the class that provide authentication method from module
    /// </summary>
    [TestClass]
    public class AuthenticationTestFromModule : AuthenticationTestBase
    {
        /// <summary>
        /// Test if empty file was supplied
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(MisconfigurationException))]
        public void TestEmptyFileConfiguration()
        {
            NameValueCollection validConfigCollection = new NameValueCollection()
            {
                { AuthenticationData.TypeKey, AuthenticationMethodProvider.AuthenticationType.SymmetricKey.ToString() },
                { AuthenticationData.IdentityKey, AuthenticationMethodProvider.AuthenticationIdentity.Device.ToString() },
                { AuthenticationData.CertificateLocationKindKey, AuthenticationMethodProvider.CertificateLocation.LocalFile.ToString() },
                { AuthenticationData.FilePathKey, "" },
                { AuthenticationData.DeviceIdKey, "testDevice" },
                { AuthenticationData.GatewayHostNameKey, "testGateway" }
            };
            UpdateAuthenticationData(new AuthenticationData(validConfigCollection));
            InitAuthenticationMethodProvider();
        }
    }
}
