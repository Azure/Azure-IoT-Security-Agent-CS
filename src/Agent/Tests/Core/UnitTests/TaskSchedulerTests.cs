// <copyright file="TaskSchedulerTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
using Microsoft.Azure.IoT.Agent.Core.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Microsoft.Azure.IoT.Agent.Core.Tests.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="TaskScheduler"/>
    /// </summary>
    [TestClass]
    public class TaskSchedulerTests
    {
        private CancellationTokenSource _cancellationToken;
        private TaskScheduler _scheduler;

        /// <summary>
        /// Test initialization
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _cancellationToken = new CancellationTokenSource();
            _scheduler = new TaskScheduler(_cancellationToken.Token);
        }

        /// <summary>
        /// Tests for <see cref="TaskScheduler.Start"/> method
        /// </summary>
        [TestClass]
        public class StartMethod : TaskSchedulerTests
        {
            /// <summary>
            /// Should execute tasks in the current thread if requested
            /// </summary>
            [TestMethod]
            [Timeout(60000)]
            public void ShouldExecuteTaskInCurrentThread()
            {
                int executingThreadId = 0;

                _scheduler.AddTask(new ActionTask(() => 
                {
                    executingThreadId = Thread.CurrentThread.ManagedThreadId;
                    _cancellationToken.Cancel();
                }), TimeSpan.Zero, DateTime.UtcNow);

                _scheduler.Start(true);

                _cancellationToken.Token.WaitHandle.WaitOne();
                Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, executingThreadId);
            }

            /// <summary>
            /// Should execute tasks in a separate worker thread if requested
            /// </summary>
            [TestMethod]
            [Timeout(60000)]
            public void ShouldExecuteTaskInSeparateThread()
            {
                int executingThreadId = 0;

                _scheduler.AddTask(new ActionTask(() =>
                {
                    executingThreadId = Thread.CurrentThread.ManagedThreadId;
                    _cancellationToken.Cancel();

                }), TimeSpan.Zero, DateTime.UtcNow);

                _scheduler.Start(false);

                _cancellationToken.Token.WaitHandle.WaitOne();
                Assert.AreNotEqual(Thread.CurrentThread.ManagedThreadId, executingThreadId);
            }

            /// <summary>
            /// Should skip tasks that their execution time haven't arrived yet
            /// </summary>
            [TestMethod]
            public void ShouldSkipTaskIfShouldNotExecute()
            {
                int executionCount = 0;

                _scheduler.AddTask(new ActionTask(() =>
                {
                    Interlocked.Increment(ref executionCount);

                }), TimeSpan.Zero, DateTime.UtcNow + TimeSpan.FromHours(5));

                _scheduler.Start(false);

                Thread.Sleep(1000);
                _cancellationToken.Cancel();

                Assert.AreEqual(0, executionCount);
            }

            /// <summary>
            /// Should keep executing tasks after tasks throw exceptions
            /// </summary>
            [TestMethod]
            public void ShouldKeepExecutionAfterTaskThrowsAnException()
            {
                int executionCount = 0;

                _scheduler.AddTask(new ActionTask(() =>
                {
                    executionCount++;

                    if (executionCount < 5)
                        throw new Exception();
                    else
                        _cancellationToken.Cancel();

                }), TimeSpan.Zero, DateTime.UtcNow);

                _scheduler.Start(true);

                Assert.AreEqual(5, executionCount);
            }
        }
    }
}
