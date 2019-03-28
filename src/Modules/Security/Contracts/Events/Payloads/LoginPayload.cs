// <copyright file="LoginPayload.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.Azure.Security.IoT.Contracts.Events.Payloads
{
    /// <summary>
    /// a login event payload
    /// </summary>
    public class LoginPayload : Payload
    {
        /// <summary>
        /// Represents a result of a login attempt
        /// </summary>
        public enum LoginResult
        {
#pragma warning disable CS1591
            Fail,
            Success
#pragma warning restore CS1591
        }

        /// <summary>
        /// The login process Id
        /// </summary>
        public uint ProcessId { get; set; }

        /// <summary>
        /// The login user Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The login user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The login operation
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// The login executable
        /// </summary>
        public string Executable { get; set; }

        /// <summary>
        /// The remote address of the login attempt
        /// </summary>
        public string RemoteAddress { get; set; }

        /// <summary>
        /// The remote port of the login attempt
        /// </summary>
        public string RemotePort { get; set; }

        /// <summary>
        /// The local port of the login attempt
        /// </summary>
        public string LocalPort { get; set; }

        /// <summary>
        /// The result of the login attempt
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LoginResult Result { get; set; }
    }
}