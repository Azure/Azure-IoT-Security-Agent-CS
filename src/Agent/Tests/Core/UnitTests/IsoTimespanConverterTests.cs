// <copyright file="IsoTimespanConverterTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="IsoTimespanConverter"/>
    /// </summary>
    [TestClass]
    public class IsoTimespanConverterTests
    {
        private IsoTimespanConverter _converter;

        /// <summary>
        /// Test initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _converter = new IsoTimespanConverter();
        }

        /// <summary>
        /// Tests for the <see cref="IsoTimespanConverter.CanConvert"/> method
        /// </summary>
        [TestClass]
        public class CanConvertMethod : IsoTimespanConverterTests
        {
            /// <summary>
            /// Should be able to convert <see cref="TimeSpan"/>
            /// </summary>
            [TestMethod]
            public void ShouldSupportTimeSpan()
            {
                Assert.IsTrue(_converter.CanConvert(typeof(TimeSpan)));
            }
        }

        /// <summary>
        /// Tests for the <see cref="IsoTimespanConverter.CanRead"/> property
        /// </summary>
        [TestClass]
        public class CanReadProperty : IsoTimespanConverterTests
        {
            /// <summary>
            /// Should be able to support read operations
            /// </summary>
            [TestMethod]
            public void ShouldSupportRead()
            {
                Assert.IsTrue(_converter.CanRead);
            }
        }

        /// <summary>
        /// Tests for the <see cref="IsoTimespanConverter.CanConvert"/> property
        /// </summary>
        [TestClass]
        public class CanWriteProperty : IsoTimespanConverterTests
        {
            /// <summary>
            /// Should be able to support write operations
            /// </summary>
            [TestMethod]
            public void ShouldSupportWrite()
            {
                Assert.IsTrue(_converter.CanWrite);
            }
        }

        /// <summary>
        /// Tests for the <see cref="IsoTimespanConverter.ReadJson"/> method
        /// </summary>
        [TestClass]
        public class ReadJsonMethod : IsoTimespanConverterTests
        {
            /// <summary>
            /// Should be able to deserialize ISO8601 formatted durations
            /// </summary>
            [TestMethod]
            public void ShouldDeserializeCorrectlyIso8601Durations()
            {
                Assert.AreEqual(TimeSpan.FromHours(10), JsonConvert.DeserializeObject<TimeSpan>("\"PT10H\"", _converter));
                Assert.AreEqual(TimeSpan.FromMinutes(10), JsonConvert.DeserializeObject<TimeSpan>("\"PT10M\"", _converter));
                Assert.AreEqual(TimeSpan.FromSeconds(10), JsonConvert.DeserializeObject<TimeSpan>("\"PT10S\"", _converter));
                Assert.AreEqual(TimeSpan.FromDays(1), JsonConvert.DeserializeObject<TimeSpan>("\"P1D\"", _converter));
                Assert.AreEqual(new TimeSpan(10, 5, 2), JsonConvert.DeserializeObject<TimeSpan>("\"PT10H5M2S\"", _converter));
            }
        }

        /// <summary>
        /// Tests for the <see cref="IsoTimespanConverter.WriteJson"/> method
        /// </summary>
        [TestClass]
        public class WriteJsonMethod : IsoTimespanConverterTests
        {
            /// <summary>
            /// Should be able to serialize to ISO8601 formatted durations
            /// </summary>
            [TestMethod]
            public void ShouldSerializeCorrectlyIso8601Durations()
            {
                Assert.AreEqual("\"PT10H\"", JsonConvert.SerializeObject(TimeSpan.FromHours(10), _converter));
                Assert.AreEqual("\"PT10M\"", JsonConvert.SerializeObject(TimeSpan.FromMinutes(10), _converter));
                Assert.AreEqual("\"PT10S\"", JsonConvert.SerializeObject(TimeSpan.FromSeconds(10), _converter));
                Assert.AreEqual("\"P1D\"", JsonConvert.SerializeObject(TimeSpan.FromDays(1), _converter));
                Assert.AreEqual("\"PT10H5M2S\"", JsonConvert.SerializeObject(new TimeSpan(10, 5, 2), _converter));
            }
        }
    }
}
