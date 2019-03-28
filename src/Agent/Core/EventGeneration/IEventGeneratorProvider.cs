// <copyright file="IEventGeneratorProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Contracts.Events;
using System.Collections.Generic;

namespace Microsoft.Azure.IoT.Agent.Core.EventGeneration
{
    /// <summary>
    /// An interface for a class that provides a list of event generators, intended to be implemented on various platforms
    /// </summary>
    public interface IEventGeneratorProvider
    {
        /// <summary>
        /// Get all events generators
        /// </summary>
        /// <returns>List of event generators</returns>
        List<IEventGenerator> GetAll();
    }
}