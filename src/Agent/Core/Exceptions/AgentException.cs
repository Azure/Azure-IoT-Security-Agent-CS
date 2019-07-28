// <copyright file="AgentException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Exceptions
{
    /// <summary>
    /// Base class for agent fatal exceptions
    /// </summary>
    public class AgentException : Exception
    {
        /// <summary>
        /// Exception code
        /// </summary>
        public ExceptionCodes ExceptionCode { get; }

        /// <summary>
        /// Exception sub code
        /// </summary>
        public ExceptionSubCodes ExceptionSubCode { get; }

        /// <summary>
        /// Exception extran info
        /// </summary>
        public string ExtraInfo { get; }

        /// <inheritdoc />
        public override string Message => $"Error in: {ExceptionCode.GetDescription()}, reason: {ExceptionSubCode.GetDescription()}, extra details: {ExtraInfo}";

        /// <summary>
        /// C-tor
        /// </summary>
        /// <param name="exceptionCode">Exception code</param>
        /// <param name="exceptionSubCode">Exception sub code</param>
        /// <param name="extraInfo">Exception extra info</param>
        public AgentException(ExceptionCodes exceptionCode, ExceptionSubCodes exceptionSubCode, string extraInfo = default(string))
        {
            ExceptionCode = exceptionCode;
            ExceptionSubCode = exceptionSubCode;
            ExtraInfo = extraInfo;
        }
    }
}
