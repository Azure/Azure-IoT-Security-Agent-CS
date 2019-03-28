// <copyright file="TaskScheduler.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Microsoft.Azure.IoT.Agent.Core.Configuration;
using Microsoft.Azure.IoT.Agent.Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.IoT.Agent.Core.Scheduling
{
    /// <summary>
    /// A slim and simple task scheduler
    /// </summary>
    public class TaskScheduler
    {
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Lock object for the task list
        /// </summary>
        private readonly object _listLock = new object();

        /// <summary>
        /// The list of tasks to execute
        /// </summary>
        private readonly List<TaskState> _tasks = new List<TaskState>();

        /// <summary>
        /// The main thread of the task scheduler (if used for multi-threaded execution)
        /// </summary>
        private Thread _taskSchedulerThread;

        /// <summary>
        /// Create a task scheduler with a cancellation token
        /// </summary>
        public TaskScheduler(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Add new task to the list of executing tasks
        /// This could get locked for some time if the scheduler is already running, but we don't expect that to happen
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="interval">The interval between each task execution</param>
        /// <param name="timeToStart">When to start the first execution of the task</param>
        public void AddTask(ITask task, TimeSpan interval, DateTime timeToStart)
        {
            lock (_listLock)
            {
                TaskState state = new TaskState(task, interval, timeToStart);
                _tasks.Add(state);
            }
        }

        /// <summary>
        /// Start the task scheduler thread
        /// </summary>
        public void Start(bool runInCurrentThread = false)
        {
            if (runInCurrentThread)
            {
                ExecuteTasks();
            }
            else
            {
                _taskSchedulerThread = new Thread(ExecuteTasks);
                _taskSchedulerThread.Start();
            }
        }

        /// <summary>
        /// Check if the tasks in the list should and execute them
        /// </summary>
        private void ExecuteTasks()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                lock (_listLock)
                {
                    foreach (TaskState state in _tasks)
                    {
                        if (state.ShouldExecute())
                        {
                            ThreadContext.Set(new ThreadContext());
                            try
                            {
                                state.ExecuteTask();
                            }
                            catch (Exception ex)
                            {
                                SimpleLogger.Error("Execution of a task failed", exception: ex);
                            }
                        }
                    }
                }

                _cancellationToken.WaitHandle.WaitOne(LocalConfiguration.General.SchedulerInterval);
            }

            SimpleLogger.Debug("Cancellation was requested, stopping scheduler");
        }
    }
}