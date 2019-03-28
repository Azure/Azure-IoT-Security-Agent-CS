using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Tests.Common.Helpers
{
    /// <summary>
    /// Extension methods for the <see cref="string"/> class
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string representing a <see cref="byte"/> array in HEX form to an actual <see cref="byte"/> array
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexToByteArray(this string hexString)
        {
            byte[] bytes = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                string s = hexString.Substring(i, 2);
                bytes[i / 2] = byte.Parse(s, NumberStyles.HexNumber, null);
            }

            return bytes;
        }
    }
}
