// <copyright file="AuthenticationTestBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using Agent.Tests.Common.UnitTests.Authentication;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.Azure.IoT.Agent.IoT.RestApis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using Microsoft.Azure.IoT.Agent.IoT.Exceptions;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests.Authentication
{
    /// <summary>
    /// Test the base functionality of AuthenticationMethodProvider
    /// </summary>
    [TestClass]
    public class AuthenticationTestBase
    {
        private const string TestGatweay = "testGateway.test.test";

        /// <summary>
        /// Initialize the authentication configuration
        /// </summary>
        [TestInitialize]
        public virtual void Init()
        {
            NameValueCollection collection = new NameValueCollection()
            {
                { AuthenticationData.TypeKey, AuthenticationMethodProvider.AuthenticationType.SymmetricKey.ToString() },
                { AuthenticationData.ModuleNameKey, "TestModule" },
                { AuthenticationData.IdentityKey, AuthenticationMethodProvider.AuthenticationIdentity.Device.ToString() },
                { AuthenticationData.CertificateLocationKindKey, AuthenticationMethodProvider.CertificateLocation.LocalFile.ToString() },
                { AuthenticationData.FilePathKey, "\var\tmp" },
                { AuthenticationData.DeviceIdKey, "testDevice" },
                { AuthenticationData.GatewayHostNameKey, TestGatweay }
            };
            _authData = new AuthenticationData(collection);
        }

         /// <summary>
        /// Update the test with the given AuthenticationData
        /// </summary>
        /// <param name="authenticationData"></param>
        protected void UpdateAuthenticationData(AuthenticationData authenticationData)
        {
            _authData = authenticationData;
        }
        
        /// <summary>
        /// Test for configuration that are not supported.
        /// Currently Module authentication does not support certificate 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(AgentException))]
        public void TestNotSupportedConfiguration()
        {
            NameValueCollection invalidConfigCollection = new NameValueCollection()
            {
                { AuthenticationData.TypeKey, AuthenticationMethodProvider.AuthenticationType.SelfSignedCertificate.ToString() },
                { AuthenticationData.ModuleNameKey, "TestModule" },
                { AuthenticationData.IdentityKey, AuthenticationMethodProvider.AuthenticationIdentity.Module.ToString() },
                { AuthenticationData.CertificateLocationKindKey, AuthenticationMethodProvider.CertificateLocation.LocalFile.ToString() },
                { AuthenticationData.FilePathKey, "\var\tmp" },
                { AuthenticationData.DeviceIdKey, "testDevice" },
                { AuthenticationData.GatewayHostNameKey, TestGatweay }
            };
            UpdateAuthenticationData(new AuthenticationData(invalidConfigCollection));
            InitAuthenticationMethodProvider();

            //VerifyInvalidConfiguration(invalidConfigCollection);
        }

        /// <summary>
        /// Test that File path is not empty
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyFileConfigurationFromDevice()
        {
            NameValueCollection invalidConfigCollection = new NameValueCollection()
            {
                { AuthenticationData.TypeKey, AuthenticationMethodProvider.AuthenticationType.SelfSignedCertificate.ToString() },
                { AuthenticationData.ModuleNameKey, "TestModule" },
                { AuthenticationData.IdentityKey, AuthenticationMethodProvider.AuthenticationIdentity.Module.ToString() },
                { AuthenticationData.CertificateLocationKindKey, AuthenticationMethodProvider.CertificateLocation.LocalFile.ToString() },
                { AuthenticationData.FilePathKey, "" },
                { AuthenticationData.DeviceIdKey, "testDevice" },
                { AuthenticationData.GatewayHostNameKey, TestGatweay }
            };
            UpdateAuthenticationData(new AuthenticationData(invalidConfigCollection));
            InitAuthenticationMethodProvider();

            //VerifyInvalidConfiguration(invalidConfigCollection);
        }

        /// <summary>
        /// Verify that the module connection string is as expected
        /// </summary>
        [TestMethod]
        public void VerifyModuleConnectionString()
        {
            string connectionString = InitAuthenticationMethodProvider();
            StringAssert.Contains(connectionString, "myTestPrimaryKey");
        }

        /// <summary>
        /// Imitate the AuthenticationMethodProvider::ProvideModuleConnectionString behavior
        /// </summary>
        /// <returns>Connection string of the Security Module</returns>
        protected string InitAuthenticationMethodProvider()
        {
            AuthenticationMethodProviderBase authenticationMethodProvider = null;
            IDeviceApi deviceApiMocked = new DeviceApiMock();
            if (_authData.Identity == AuthenticationMethodProvider.AuthenticationIdentity.Module)
            {
                authenticationMethodProvider = new AuthenticationMethodProviderFromModule(_authData);
            }
            else if(_authData.Identity == AuthenticationMethodProvider.AuthenticationIdentity.Device)
            {
                authenticationMethodProvider = new AuthenticationMethodProviderFromDevice(deviceApiMocked, _authData);
            }
            else
            {
                throw new ArgumentOutOfRangeException(paramName: "identity", message: "Not supported identity");
            }

            var connectionString = authenticationMethodProvider.GetConnectionString();
            return connectionString;
        }
        private AuthenticationData _authData;
    }
}