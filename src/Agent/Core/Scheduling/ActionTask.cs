// <copyright file="ActionTask.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Scheduling
{
    /// <summary>
    /// A generic task to use with the task scheduler
    /// </summary>
    public class ActionTask : ITask
    {
        private readonly Action _action;

        /// <summary>
        /// Ctor, constructs a generic task
        /// </summary>
        /// <param name="action">The action this task executes</param>
        public ActionTask(Action action)
        {
            _action = action;
        }

        /// <inheritdoc />
        public void ExecuteTask()
        {
            _action.Invoke();
        }
    }
}