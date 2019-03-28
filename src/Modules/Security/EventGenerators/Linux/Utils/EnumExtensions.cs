// <copyright file="EnumExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Utils
{
    /// <summary>
    /// Enum extension methods
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get an attribute for a specific enum value
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute</typeparam>
        /// <param name="enumValue">The enum value</param>
        /// <returns>The attribute</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// Parse a string to an enum according to display attributes defined on the enum values.
        /// Display attributes must be defined on all members of the enum
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum</typeparam>
        /// <param name="value">The value to parse</param>
        /// <returns>The enum value</returns>
        public static Enum ParseFromDisplayName<TEnum>(this string value)
        {
            foreach (Enum item in Enum.GetValues(typeof(TEnum)))
            {
                if (value.Equals(item.GetAttribute<DisplayAttribute>().Name))
                {
                    return item;
                }
            }

            throw new ArgumentException("could no parse value " + value);
        }
    }
}