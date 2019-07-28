// <copyright file="EventExtensions.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.MessageWorker;
using Microsoft.Azure.IoT.Contracts.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Security.Tests.Common.Helpers
{
    /// <summary>
    /// Extensions for the IEvent class to be used in tests
    /// </summary>
    public static class EventExtensions
    {
        private static readonly string _defaultSchemaLocation = GetDefaultSchemaLocation();

        /// <summary>
        /// Validate an event against the schema
        /// </summary>
        /// <param name="ev">the event</param>
        /// <param name="schemaPath">optional schema path override</param>
        public static void ValidateSchema(this IEvent ev, string schemaPath = null)
        {
            AgentMessage message = new AgentMessage(new[] { ev }, Guid.NewGuid(), "1.0.0");
            ValidateContent(message, schemaPath ?? _defaultSchemaLocation);
        }

        /// <summary>
        /// Validate multiple events against the schema
        /// </summary>        
        /// <param name="events">the events to validate</param>
        /// <param name="schemaPath">optional schema path override</param>
        public static void ValidateSchema(this IEnumerable<IEvent> events, string schemaPath = null)
        {
            events.ToList().ForEach(ev => ev.ValidateSchema(schemaPath ?? _defaultSchemaLocation));
        }

        private static string GetDefaultSchemaLocation()
        {
            var currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            return Path.Combine(GetAncestorDirectory(currentLocation, 8), "Azure-IoT-Security/security_message/schemas", "messageRoot.json");
        }

        private static string GetAncestorDirectory(string directory, int levelsToGoUp)
        {
            if (levelsToGoUp == 0)
                return directory;

            return Directory.GetParent(GetAncestorDirectory(directory, levelsToGoUp-1)).FullName;
        }

        private static void ValidateContent(AgentMessage objectToValidate, string schemaPath)
        {
            using (StreamReader schemaFile = File.OpenText(schemaPath))
            using (JsonTextReader schemaReader = new JsonTextReader(schemaFile))
            {
                JSchema schema = JSchema.Load(schemaReader, new JSchemaReaderSettings
                {
                    Resolver = new JSchemaUrlResolver(),
                    BaseUri = new Uri(schemaPath)
                });

                string serialized = JsonConvert.SerializeObject(objectToValidate, JsonFormatting.SerializationSettings);
                JObject token = JObject.Parse(serialized);

                IList<ValidationError> errors;
                bool isValid = token.IsValid(schema, out errors);

                var relevantErrors = FilterBySchemaFile(errors, objectToValidate.Events.First().Name, "TypeDefinitions");
                relevantErrors.ToList().ForEach(err => Debug.WriteLine($"Validation error: {err.Message}"));

                Assert.IsTrue(isValid);  
            }
        }

        private static IEnumerable<ValidationError> FilterBySchemaFile(IList<ValidationError> errors, params string[] filters)
        {
            List<ValidationError> filteredErrors = new List<ValidationError>();

            foreach ( var childError in errors)
            {
                if (childError.ChildErrors.Count == 0)
                {
                    string fileName = Path.GetFileName(childError.SchemaBaseUri.AbsolutePath);

                    foreach (string filter in filters)
                    {
                        if (fileName.Contains(filter))
                            filteredErrors.Add(childError);
                    }
                }

                filteredErrors.AddRange(FilterBySchemaFile(childError.ChildErrors, filters));
            }

            return filteredErrors;
        }
    }
}
