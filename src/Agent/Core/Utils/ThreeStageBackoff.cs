using Microsoft.Azure.Devices.Client;
using System;

namespace Microsoft.Azure.IoT.Agent.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ThreeStageBackoff : IRetryPolicy
    {
        /// <summary>
        /// A retry policy which retries indefinitely with 3 stages of backoff durations.
        /// </summary>
        /// <param name="firstStageCount">Number of tries for the first stage</param>
        /// <param name="firstStageDuration">Retry interval for the first stage</param>
        /// <param name="secondStageCount">Number of tries for the second stage</param>
        /// <param name="secondStageDuration">Retry interval for the second stage</param>
        /// <param name="thirdStageDuration">Retry interval for the third stage</param>
        public ThreeStageBackoff(int firstStageCount, TimeSpan firstStageDuration, int secondStageCount, TimeSpan secondStageDuration, TimeSpan thirdStageDuration)
        {
            _firstStageCount = firstStageCount;
            _firstStageDuration = firstStageDuration;
            _secondStageCount = secondStageCount;
            _secondStageDuration = secondStageDuration;
            _thirdStageDuration = thirdStageDuration;
        }

        /// <inheritdoc />
        public bool ShouldRetry(int currentRetryCount, Exception lastException, out TimeSpan retryInterval)
        {
            if (retryCount < _firstStageCount)
            {
                retryInterval = _firstStageDuration;
            } else if (retryCount < _firstStageCount + _secondStageCount)
            {
                retryInterval = _secondStageDuration;
            } else
            {
                retryInterval = _thirdStageDuration;
            }

            ++retryCount;

            return true;
        }

        private int retryCount = 0;

        private readonly int _firstStageCount;
        private readonly TimeSpan _firstStageDuration;
        private readonly int _secondStageCount;
        private readonly TimeSpan _secondStageDuration;
        private readonly TimeSpan _thirdStageDuration;
    }
}
