// <copyright file="ITask.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.Azure.IoT.Agent.Core.Scheduling
{
    /// <summary>
    /// Interface for a task in the task scheduler
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Executes the task, should be called by the task scheduler
        /// </summary>
        void ExecuteTask();
    }
}