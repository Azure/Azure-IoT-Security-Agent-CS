// <copyright file="ConfigurationUtilsTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.Azure.IoT.Agent.Core.Utils;

namespace Security.Tests.Common.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="ConfigurationUtils"/>
    /// </summary>
    [TestClass]
    public class ConfigurationUtilsTests
    {
        /// <summary>
        /// Tests for <see cref="ConfigurationUtils.AppendToProcessPath(string)"/>
        /// </summary>
        [TestClass]
        public class AppendToProcessPathMethod : ConfigurationUtilsTests
        {
            /// <summary>
            /// Returned path should be absolute and relative to the process directory
            /// </summary>
            [TestMethod]
            public void ShouldReturnAbsolutePathRelativeToProcessDirectory()
            {
                string relativePath = "a.txt";
                string expectedFullPath = Path.GetFullPath(relativePath);

                // Not using IoC so simulate a scenario
                string originalCurrentDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = Path.GetTempPath();

                try
                {
                    Assert.AreEqual(expectedFullPath, ConfigurationUtils.AppendToProcessPath(relativePath));
                }
                finally
                {
                    Environment.CurrentDirectory = originalCurrentDirectory;
                }
            }
        }

        /// <summary>
        /// Tests for <see cref="ConfigurationUtils.IsFullyQualifiedPath(string)"/>
        /// </summary>
        [TestClass]
        public class TryGetProcessFullPathMethod : ConfigurationUtilsTests
        {
            /// <summary>
            /// Expected true for fully qualified paths
            /// </summary>
            [TestMethod]
            public void ShouldReturnTrueIfPathIsFullyQualified()
            {
                string fullPath = Path.GetFullPath("a.txt");

                Assert.IsTrue(ConfigurationUtils.IsFullyQualifiedPath(fullPath));
            }

            /// <summary>
            /// Expected false for relative paths
            /// </summary>
            [TestMethod]
            public void ShouldReturnFalseIfPathIsRelative()
            {
                 Assert.IsFalse(ConfigurationUtils.IsFullyQualifiedPath("a.txt"));
            }

            /// <summary>
            /// Expected failure for uppercase fully qualified paths
            /// </summary>
            [TestMethod]
            public void ShouldReturnTrueIfPathIsUppercaseFullyQualified()
            {
                string uppercaseFullPath = Path.GetFullPath("a.txt").ToUpper();

                Assert.IsTrue(ConfigurationUtils.IsFullyQualifiedPath(uppercaseFullPath));
            }
        }
    }
}
