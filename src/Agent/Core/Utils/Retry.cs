using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Azure.Devices.Client;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// Used to retry an action according to a predefined retry policy.
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// Runs a given action according to a retry policy.
        /// </summary>
        /// <param name="action">The action to run</param>
        /// <param name="retryPolicy">The retry policy</param>
        public static void Do(
            Action action,
            IRetryPolicy retryPolicy)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryPolicy);
        }

        /// <summary>
        /// Runs a given action according to a retry policy with return value.
        /// </summary>
        /// <typeparam name="T">The action's return type</typeparam>
        /// <param name="action">The action to run</param>
        /// <param name="retryPolicy">The retry policy</param>
        /// <returns></returns>
        public static T Do<T>(
            Func<T> action,
            IRetryPolicy retryPolicy)
        {
            var exceptions = new List<Exception>();
            int retryCount = 0;
            Exception lastException = null;
            TimeSpan retryInterval = TimeSpan.FromSeconds(0);

            while (retryPolicy.ShouldRetry(retryCount, lastException, out retryInterval))
            {
                try
                {
                    Thread.Sleep(retryInterval);
                    return action();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    exceptions.Add(ex);
                }
            }
            throw new AggregateException(exceptions);
        }
    }
}
