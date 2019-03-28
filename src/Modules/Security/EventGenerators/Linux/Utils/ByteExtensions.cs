// <copyright file="ByteExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.Linq;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils
{
    /// <summary>
    /// Extension methods for byte array
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        /// Converts the first two bytes of a bytearray from the given offset
        /// to a ushort.
        /// The conversion revereses the byte order if isDataLittleEndian != SystemArchitecture.IsLittleEndian
        /// </summary>
        /// <param name="bytes">Bytes array to convert</param>
        /// <param name="offset">Offset to start from</param>
        /// <param name="isDataLittleEndian">Indicates if the data in the array is in littleEndian</param>
        /// <returns>ushort</returns>
        public static ushort ToUshort(this byte[] bytes, int offset, bool isDataLittleEndian)
        {
            var bytesToConvert = bytes.ToSystemArchitectureEndian(offset, 2, isDataLittleEndian);

            return BitConverter.ToUInt16(bytesToConvert, 0);
        }

        /// <summary>
        /// Converts the first two bytes of a bytearray from the given offset
        /// to a uint.
        /// The conversion revereses bytes order if isDataLittleEndian != SystemArchitecture.IsLittleEndian
        /// </summary>
        /// <param name="bytes">Bytes array to convert</param>
        /// <param name="offset">Offset to start from</param>
        /// <param name="isDataLittleEndian">Indicates if the data in the array is in littleEndian</param>
        /// <returns>uint from the first four bytes starting at offset</returns>
        public static uint ToUint(this byte[] bytes, int offset, bool isDataLittleEndian)
        {
            var bytesToConvert = bytes.ToSystemArchitectureEndian(offset, 4, isDataLittleEndian);

            return BitConverter.ToUInt32(bytesToConvert, 0);
        }

        /// <summary>
        /// Creates a sub array of bytes
        /// the sub array is the first size bytes from offset: offset in the byte array bytes.
        /// If isDataLittleEndian != SystemArchitecture.IsLittleEndian, the method reverese the array order.
        /// </summary>
        /// <param name="bytes">Byte array to convert</param>
        /// <param name="offset">Offset in the array to start from</param>
        /// <param name="size">Amount of bytes to convert</param>
        /// <param name="isDataLittleEndian">Is data in the array is littleEndian</param>
        /// <returns>byte array in system architecture endianess</returns>
        public static byte[] ToSystemArchitectureEndian(this byte[] bytes, int offset, int size, bool isDataLittleEndian)
        {
            var byteArray = bytes.Skip(offset).Take(size);
            if (isDataLittleEndian && BitConverter.IsLittleEndian ||
                !isDataLittleEndian && !BitConverter.IsLittleEndian)
            {
                return byteArray.ToArray();
            }

            return byteArray.Reverse().ToArray();
        }

        /// <summary>
        /// Convert hex string to byte array
        /// </summary>
        /// <param name="hex">Hex string to convert</param>
        /// <returns></returns>
        public static byte[] GetBytesFromFromHexString(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
    }
}