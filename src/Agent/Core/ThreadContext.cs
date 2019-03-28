// <copyright file="ThreadContext.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System;

namespace Microsoft.Azure.IoT.Agent.Core
{
    /// <summary>
    /// Represents thread execution context
    /// </summary>
    public class ThreadContext
    {
        [ThreadStatic]
        private static ThreadContext _currentContext;

        /// <summary>
        /// Get the current thread execution context
        /// </summary>
        public static ThreadContext Get()
        {
            _currentContext = _currentContext ?? new ThreadContext();

            return _currentContext;
        }

        /// <summary>
        /// Set the current thread execution context
        /// </summary>
        /// <param name="threadContext">Thread context to set</param>
        public static void Set(ThreadContext threadContext)
        {
            _currentContext = threadContext;
        }

        /// <summary>
        /// Context execution id
        /// </summary>
        public Guid ExecutionId { get; }

        /// <summary>
        /// Creates a new context
        /// </summary>
        public ThreadContext()
        {
            ExecutionId = Guid.NewGuid();
        }
    }
}