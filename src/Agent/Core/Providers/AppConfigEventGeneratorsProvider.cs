// <copyright file="AppConfigEventGeneratorsProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.EventGeneration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using Microsoft.Azure.IoT.Contracts.Events;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Azure.IoT.Agent.Core.Providers
{
    /// <summary>
    /// A provider class that provides all registered event generators from the App.Config file.
    /// </summary>
    public class AppConfigEventGeneratorsProvider : EventGeneratorProviderBase
    {
        /// <inheritdoc />
        protected override void Load()
        {
            if (!IsLoaded)
            {
                var eventGeneratorToLoad = LocalConfiguration.EventGenerators;
                var cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                foreach (var eg in eventGeneratorToLoad)
                {
                    try
                    {
                        SimpleLogger.Information($"Loading Event Generator: {eg.Name}, from DLL: {eg.Dll}");
                        var absolutePath = Path.IsPathRooted(eg.Dll) ? eg.Dll : Path.Combine(cwd, eg.Dll); 
                        Assembly assembly = Assembly.LoadFrom(absolutePath);
                        Type type = assembly.GetType(eg.Name);
                        EventGenerators.Add((IEventGenerator) Activator.CreateInstance(type));
                    }
                    catch (Exception e)
                    {
                        SimpleLogger.Error($"Couldn't load Event Generator: {eg.Name}, from DLL: {eg.Dll}. Exception {e}");
                    }
                }
            }

            IsLoaded = true;
        }
    }
}