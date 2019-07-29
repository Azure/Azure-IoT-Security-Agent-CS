using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Azure.IoT.Agent.Core.Exceptions;

namespace Microsoft.Azure.IoT.Agent.IoT.AuthenticationUtils
{
    /// <summary>
    /// File utilities for Authentication classes
    /// </summary>
    public static class AuthenticationFileUtils
    {
        /// <summary>
        /// Read a symmetric key string from a given file
        /// </summary>
        /// <param name="filePath">file path for the symmetric key</param>
        /// <returns>Symmetric key as it was provided in the File Path</returns>
        /// <exception cref="ArgumentOutOfRangeException">In case the file is empty</exception>
        public static string GetBase64EncodedSymmetricKeyFromFile(string filePath)
        {
            //Read the connection string from the file:
            string content = GetFileContent(filePath).Trim();
            if (!content.IsBase64String())
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.FileFormat, "Key is not base 64 encoded");
            }

            return content;
        }

        /// <summary>
        /// Gets the contents of the given file as a string
        /// </summary>
        /// <param name="filePath">a path to the file</param>
        /// <returns>file content as string</returns>
        public static string GetFileContent(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (IOException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.FileNotExist, $"File not found in path: {filePath}");
            }
            catch (UnauthorizedAccessException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.FilePermissions, $"Couldn't open file in path: {filePath}, check permissions");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException) 
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, $"Unexpected error while opening file: {filePath}");
            }
        }

        /// <summary>
        /// Gets the contents of the given file as a byte array
        /// </summary>
        /// <param name="filePath">a path to the file</param>
        /// <returns>file content</returns>
        public static byte[] GetBinaryFileContent(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (IOException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.FileNotExist, $"File not found in path: {filePath}");
            }
            catch (UnauthorizedAccessException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.FilePermissions, $"Couldn't open file in path: {filePath}, check permissions");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException)
            {
                throw new AgentException(ExceptionCodes.Authentication, ExceptionSubCodes.Other, $"Unexpected error while opening file: {filePath}");
            }
        }

        private static bool IsBase64String(this string s)
        {
            Regex base64 = new Regex(@"^[a-zA-Z0-9\+/]*={0,3}$");
            return (s.Length % 4 == 0) && base64.IsMatch(s);
        }
    }
}
