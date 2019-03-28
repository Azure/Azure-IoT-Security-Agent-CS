// <copyright file="RemoteSecurityModuleConfigurationTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.Azure.Security.IoT.Agent.Common;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Security.Tests.Common
{
    /// <summary>
    /// Unit tests for <see cref="RemoteSecurityModuleConfiguration"/>
    /// </summary>
    [TestClass]
    public class RemoteSecurityModuleConfigurationTests
    {
        /// <summary>
        /// Verify that <see cref="RemoteSecurityModuleConfiguration"/> have all of the core and security events defined correctly
        /// </summary>
        [TestMethod]
        public void ShouldHaveAllSupportedEventsDefinedCorrectly()
        {
            Type baseEventType = typeof(IEvent);
            var eventAssemblies = new Assembly[] { typeof(Login).Assembly, typeof(IEvent).Assembly };

            foreach (Assembly assembly in eventAssemblies)
            {
                IEnumerable<Type> eventTypes = assembly.GetTypes().Where(type => baseEventType.IsAssignableFrom(type) && type != baseEventType && !type.IsAbstract);
                Assert.IsTrue(eventTypes.First() != null);

                Type configurationType = typeof(RemoteSecurityModuleConfiguration);

                foreach (Type eventType in eventTypes)
                {
                    PropertyInfo propertyInfo = configurationType.GetProperty(eventType.Name);

                    Assert.IsNotNull(propertyInfo);
                    Assert.AreEqual(typeof(EventPriority), propertyInfo.PropertyType);
                    Assert.IsTrue(propertyInfo.GetGetMethod().IsPublic);
                    Assert.IsTrue(propertyInfo.GetSetMethod().IsPublic);

                    IEnumerable<CustomAttributeData> customAttributes = propertyInfo.CustomAttributes;

                    CustomAttributeData defaultValueAttribute = customAttributes.First(attribute => attribute.AttributeType == typeof(DefaultValueAttribute));
                    Assert.IsNotNull(defaultValueAttribute);
                    Assert.IsTrue(defaultValueAttribute.ConstructorArguments.Any(arg => Enum.IsDefined(typeof(EventPriority), arg.Value.ToString())));

                    CustomAttributeData jsonPropertyAttirubte = customAttributes.First(attribute => attribute.AttributeType == typeof(JsonPropertyAttribute));
                    Assert.IsNotNull(jsonPropertyAttirubte);
                    Assert.IsTrue(jsonPropertyAttirubte.NamedArguments.Any(arg =>
                    {
                        if (arg.MemberName != nameof(JsonPropertyAttribute.PropertyName))
                            return false;

                        return $"eventPriority{eventType.Name}" == (string)arg.TypedValue.Value;
                    }));
                    Assert.IsTrue(jsonPropertyAttirubte.NamedArguments.Any(arg =>
                    {
                        if (arg.MemberName != nameof(JsonPropertyAttribute.DefaultValueHandling))
                            return false;

                        return DefaultValueHandling.Populate == (DefaultValueHandling)arg.TypedValue.Value;
                    }));
                    Assert.IsTrue(jsonPropertyAttirubte.NamedArguments.Any(arg =>
                    {
                        if (arg.MemberName != nameof(JsonPropertyAttribute.NullValueHandling))
                            return false;

                        return NullValueHandling.Ignore == (NullValueHandling)arg.TypedValue.Value;
                    }));
                }
            }
        }
    }
}
