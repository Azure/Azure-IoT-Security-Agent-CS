// <copyright file="EncodedAuditFieldsUtils.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit
{
    /// <summary>
    /// Utils to help with auditd records field that are encoded
    /// </summary>
    public static class EncodedAuditFieldsUtils
    {
        /// <summary>
        /// Used to determine if a string is encoded or not
        /// </summary>
        private static readonly Regex NonHexRegex = new Regex(@"[^0-9A-F]");
        private static readonly Regex IsNumberRegex = new Regex(@"^[\d\.]+$");

        /// <summary>
        /// Decode a string that is presented in an encoded way
        /// </summary>
        /// <param name="encodedString">The encoded string in hex</param>
        /// <param name="encoding">The text encoding to assume when decoding</param>
        /// <returns></returns>
        public static string DecodeHexStringIfNeeded(string encodedString, Encoding encoding)
        {
            //sometimes fields that should be encoded are not encoded because reasons ¯\_(ツ)_/¯
            //in that case, just return the original string 
            if (NonHexRegex.IsMatch(encodedString) || IsNumberRegex.IsMatch(encodedString))
            {
                return encodedString;
            }

            byte[] data = ByteExtensions.GetBytesFromFromHexString(encodedString);
            string str = encoding.GetString(data);
            return str.Replace("\0", " ");
        }
    }
}