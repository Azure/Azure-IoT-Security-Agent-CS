// <copyright file="RestClient.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils;
using Microsoft.Azure.Security.IoT.Agent.Common.Utils;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.Azure.IoT.Agent.IoT.RestApis
{
    /// <summary>
    /// RestClient implements rest communication to the IoT hub
    /// Currently support only GET request
    /// </summary>
    public class RestClient
    {
        private static readonly uint _sasTokenTtl = 10;

        /// <summary>
        /// FQDN of the gateway (IoT hub)
        /// </summary>
        public string GatewayHostname { get; }

        /// <summary>
        /// Creates a <see cref="RestClient"/> using a given <see cref="AuthenticationData"/>
        /// </summary>
        /// <param name="authenticationData">The data from which the instance will be created</param>
        public static RestClient CreateFrom(AuthenticationData authenticationData)
        {
            switch (authenticationData.Type)
            {
                case AuthenticationMethodProvider.AuthenticationType.SymmetricKey:
                    {
                        // Getting a specific module using REST with SAS token authentication
                        string key = ConfigurationUtils.GetSymmetricKeyFromFile(authenticationData.FilePath);
                        string sasToken = GenerateSaSToken(key, authenticationData.GatewayHostName, TimeSpan.FromMinutes(_sasTokenTtl));
                        var restClient = new RestClient(authenticationData.GatewayHostName, sasToken);
                        return restClient;
                    }
                case AuthenticationMethodProvider.AuthenticationType.SelfSignedCertificate:
                    {
                        X509Certificate2 cert = CertificateLoader.Load(authenticationData.CertificateLocation.Value, authenticationData.FilePath);
                        var restClient = new RestClient(authenticationData.GatewayHostName, cert);
                        return restClient;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(authenticationData.Type), authenticationData.Type, "Unsupported authentication type");
            }
        }

        /// <summary>
        /// Generate SaS token from a given connection string 
        /// </summary>
        /// <param name="key">shared access key for creating the signature</param>
        /// <param name="target">target url resource</param>
        /// <param name="ttl">TTL</param>
        /// <returns>the SAS token for authentication</returns>
        private static string GenerateSaSToken(string key, string target, TimeSpan ttl)
        {
            var sasSignatureBuidler = new SharedAccessSignatureBuilder
            {
                Key = key,
                TimeToLive = ttl,
                Target = target
            };

            string signature = sasSignatureBuidler.ToSignature();
            return signature;
        }

        /// <summary>
        /// RestClient Constructor
        /// </summary>
        /// <param name="gatewayHostname">FQDN of the gateway e.g. iotTestHub.azure.net</param>
        /// <param name="sasToken">SAS token for SAS authentication</param>
        public RestClient(string gatewayHostname, String sasToken)
        {
            GatewayHostname = gatewayHostname;
            _sasToken = sasToken;
        }

        /// <summary>
        /// RestClient Constructor
        /// </summary>
        /// <param name="gatewayHostname">Fqdn of the gateway e.g. iotTestHub.azure.net</param>
        /// <param name="x509Certficate">x509 certificate for authentication</param>
        public RestClient(string gatewayHostname, X509Certificate2 x509Certficate)
        {
            GatewayHostname = gatewayHostname;
            _certificate = x509Certficate;
        }

        /// <summary>
        /// Sends "GET" request to the gateway to the given URL
        /// </summary>
        /// <param name="url">url for the rest request</param>
        /// <returns></returns>
        public string SendGetRequest(string url)
        {
            string responseString;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = GetRequest;
            SetAuthenticationHeader(request);
            SimpleLogger.Debug("Send get request to url: " + url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))//stream cannot be null, otherwise an exception was thrown 
            {
                responseString = reader.ReadToEnd();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    SimpleLogger.Error($"Get request failed, StatusCode: {response.StatusCode}, response: {responseString}");
                }
            }
            return responseString;
        }

        /// <summary>
        /// Set the required authentication header for the rest request
        /// </summary>
        /// <param name="request">HttpWebRequest request</param>
        private void SetAuthenticationHeader(HttpWebRequest request)
        {
            if (_certificate != null) //Certificate authentication
            {
                request.ClientCertificates.Add(_certificate);
                SimpleLogger.Debug("Certificate authentication was set for the Rest request");
            }
            else if (!string.IsNullOrEmpty(_sasToken)) //Sas authentication
            {
                request.Headers.Add(HttpRequestHeader.Authorization, _sasToken);
                SimpleLogger.Debug("SaS token was set for the Rest request");
            }
        }

        private readonly X509Certificate2 _certificate;
        private readonly string _sasToken;
        private const string GetRequest = "GET";

    }
}
