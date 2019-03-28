// <copyright file="TaskState.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core.Scheduling
{
    /// <summary>
    /// Represents the state of a task in the task scheduler
    /// </summary>
    public class TaskState
    {
        /// <summary>
        /// The next execution time for this task
        /// </summary>
        private DateTime _nextExecutionTime;

        /// <summary>
        /// The task to execute
        /// </summary>
        private readonly ITask _task;

        /// <summary>
        /// The interval between two task executions
        /// </summary>
        private readonly TimeSpan _taskInterval;

        /// <summary>
        /// The time to begin executing the task
        /// </summary>
        private readonly DateTime _timeToStart;

        /// <summary>
        /// Ctor - creates a new task state object
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="interval">The interval between two task executions</param>
        /// <param name="timeToStart">When to start the first execution of the task</param>
        public TaskState(ITask task, TimeSpan interval, DateTime timeToStart)
        {
            _task = task;
            _taskInterval = interval;
            _timeToStart = timeToStart;
            CalculateNextExecutionTime();
        }

        /// <summary>
        /// Should this task get executed now
        /// </summary>
        /// <returns>True if task should execute</returns>
        public bool ShouldExecute()
        {
            if (DateTime.UtcNow > _timeToStart && DateTime.UtcNow > _nextExecutionTime)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Execute the task and update the next execution time
        /// </summary>
        public void ExecuteTask()
        {
            CalculateNextExecutionTime();
            _task.ExecuteTask();
        }

        /// <summary>
        /// Calculate the next execution time
        /// </summary>
        private void CalculateNextExecutionTime()
        {
            _nextExecutionTime = DateTime.UtcNow.Add(_taskInterval);
        }
    }
}