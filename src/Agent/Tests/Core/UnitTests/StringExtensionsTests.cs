// <copyright file="StringExtensionsTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using System;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="StringExtensions"/>
    /// </summary>
    [TestClass]
    public class StringExtensionsTests
    {
        /// <summary>
        /// Tests for <see cref="StringExtensions.SplitStringOnNewLine"/>
        /// </summary>
        [TestClass]
        public class SplitStringOnNewLineMethod
        {
            /// <summary>
            /// Should support string without line breaks
            /// </summary>
            [TestMethod]
            public void ShouldKeepOriginalStringsWithNoNewLines()
            {
                string input = "abc";
                string[] result = input.SplitStringOnNewLine();

                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(input, result[0]);
            }

            /// <summary>
            /// Should return empty collection on empty string input
            /// </summary>
            [TestMethod]
            public void ShouldReturnEmptyCollectionOnEmptyStrings()
            {
                string input = string.Empty;
                string[] result = input.SplitStringOnNewLine();

                Assert.AreEqual(0, result.Length);
            }

            /// <summary>
            /// Should split string on line breaks
            /// </summary>
            [TestMethod]
            public void ShouldSplitOnNewLines()
            {
                string input = $"a{Environment.NewLine}b{Environment.NewLine}c";

                string[] result = input.SplitStringOnNewLine();
                Assert.AreEqual(3, result.Length);
                Assert.AreEqual("a", result[0]);
                Assert.AreEqual("b", result[1]);
                Assert.AreEqual("c", result[2]);
            }

            /// <summary>
            /// Should ignore empty lines in input string
            /// </summary>
            [TestMethod]
            public void ShouldIgnoreEmptyLines()
            {
                string input = $"a{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}b{Environment.NewLine}{Environment.NewLine}c";

                string[] result = input.SplitStringOnNewLine();
                Assert.AreEqual(3, result.Length);
                Assert.AreEqual("a", result[0]);
                Assert.AreEqual("b", result[1]);
                Assert.AreEqual("c", result[2]);
            }
        }
    }
}
