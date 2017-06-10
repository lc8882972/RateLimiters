using System;
using System.Threading;
using Xunit;

namespace Bert.RateLimiters.Tests
{
    public class FixedTokenBucketTests
    {
        private FixedTokenBucket bucket = new FixedTokenBucket(MAX_TOKENS, REFILL_INTERVAL, 1000);
        public const long MAX_TOKENS = 10;
        public const long REFILL_INTERVAL = 10;
        public const long N_LESS_THAN_MAX = 2;
        public const long N_GREATER_THAN_MAX = 12;
        private const int CUMULATIVE = 2;

        //[SetUp]
        //public void SetUp()
        //{
        //    bucket = new FixedTokenBucket(MAX_TOKENS, REFILL_INTERVAL, 1000);
        //}

        [Fact]
        public void ShouldThrottle_WhenCalledWithNTokensLessThanMax_ReturnsFalse()
        {
            TimeSpan waitTime;
            var shouldThrottle = bucket.ShouldThrottle(N_LESS_THAN_MAX, out waitTime);

            Assert.False(shouldThrottle);
            Assert.Equal(bucket.CurrentTokenCount, MAX_TOKENS - N_LESS_THAN_MAX);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithNTokensGreaterThanMax_ReturnsTrue()
        {
            TimeSpan waitTime;
            var shouldThrottle = bucket.ShouldThrottle(N_GREATER_THAN_MAX, out waitTime);

            Assert.True(shouldThrottle);
            Assert.Equal(waitTime, TimeSpan.FromMilliseconds(REFILL_INTERVAL * 1000));
            Assert.Equal(bucket.CurrentTokenCount, MAX_TOKENS);
        }


        [Fact]
        public void ShouldThrottle_WhenCalledCumulativeNTimesIsLessThanMaxTokens_ReturnsFalse()
        {
            for (int i = 0; i < CUMULATIVE; i++)
            {
                TimeSpan waitTime;
                Assert.False(bucket.ShouldThrottle(N_LESS_THAN_MAX, out waitTime));
                Assert.Equal(waitTime, TimeSpan.Zero);
            }

            var tokens = bucket.CurrentTokenCount;

            Assert.Equal(tokens, MAX_TOKENS - (CUMULATIVE * N_LESS_THAN_MAX));
        }


        [Fact]
        public void ShouldThrottle_WhenCalledCumulativeNTimesIsGreaterThanMaxTokens_ReturnsTrue()
        {

            for (int i = 0; i < CUMULATIVE; i++)
            {
                Assert.True(bucket.ShouldThrottle(N_GREATER_THAN_MAX));
            }

            var tokens = bucket.CurrentTokenCount;

            Assert.Equal(tokens, MAX_TOKENS);
        }


        [Fact]
        public void ShouldThrottle_WhenCalledWithNLessThanMaxSleepNLessThanMax_ReturnsFalse()
        {
            SystemTime.SetCurrentTimeUtc = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = SystemTime.UtcNow;

            var before = bucket.ShouldThrottle(N_LESS_THAN_MAX);
            var tokensBefore = bucket.CurrentTokenCount;
            Assert.False(before);
            Assert.Equal(tokensBefore, MAX_TOKENS - N_LESS_THAN_MAX);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(REFILL_INTERVAL);

            var after = bucket.ShouldThrottle(N_LESS_THAN_MAX);
            var tokensAfter = bucket.CurrentTokenCount;
            Assert.False(after);
            Assert.Equal(tokensAfter,MAX_TOKENS - N_LESS_THAN_MAX);

        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithNGreaterThanMaxSleepNGreaterThanMax_ReturnsTrue()
        {
            SystemTime.SetCurrentTimeUtc = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = SystemTime.UtcNow;

            TimeSpan waitTime;
            var before = bucket.ShouldThrottle(N_GREATER_THAN_MAX, out waitTime);
            var tokensBefore = bucket.CurrentTokenCount;
            Assert.Equal(waitTime, TimeSpan.FromSeconds(REFILL_INTERVAL));
            Assert.True(before);
            Assert.Equal(tokensBefore ,MAX_TOKENS);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(REFILL_INTERVAL + 1);

            var after = bucket.ShouldThrottle(N_GREATER_THAN_MAX, out waitTime);
            var tokensAfter = bucket.CurrentTokenCount;
            Assert.True(after);
            Assert.Equal(waitTime, TimeSpan.FromSeconds(REFILL_INTERVAL));
            Assert.Equal(tokensAfter,MAX_TOKENS);
        }

        [Fact]
        public void ShouldThrottle_WhenThrottle_WaitTimeIsDynamicallyCalculated()
        {
            var virtualTime = new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);

            for (int i = 0; i < 3; i++)
            {
                int closureI = i;
                SystemTime.SetCurrentTimeUtc = () => virtualTime.AddSeconds(closureI * 3);
                TimeSpan waitTime;
                bucket.ShouldThrottle(N_GREATER_THAN_MAX, out waitTime);
                Assert.Equal(waitTime, TimeSpan.FromSeconds(10 - i * 3));
            }

        }


        [Fact]
        public void ShouldThrottle_WhenCalledWithNLessThanMaxSleepCumulativeNLessThanMax()
        {
            SystemTime.SetCurrentTimeUtc = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = SystemTime.UtcNow;

            long sum = 0;
            for (int i = 0; i < CUMULATIVE; i++)
            {
                Assert.False(bucket.ShouldThrottle(N_LESS_THAN_MAX));
                sum += N_LESS_THAN_MAX;
            }
            var tokensBefore = bucket.CurrentTokenCount;
            Assert.Equal(tokensBefore, (MAX_TOKENS - sum));

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(REFILL_INTERVAL);

            for (int i = 0; i < CUMULATIVE; i++)
            {
                Assert.False(bucket.ShouldThrottle(N_LESS_THAN_MAX));
            }
            var tokensAfter = bucket.CurrentTokenCount;
            Assert.Equal(tokensAfter, (MAX_TOKENS - sum));
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithCumulativeNLessThanMaxSleepCumulativeNGreaterThanMax()
        {
            SystemTime.SetCurrentTimeUtc = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = SystemTime.UtcNow;

            long sum = 0;
            for (int i = 0; i < CUMULATIVE; i++)
            {
                Assert.False(bucket.ShouldThrottle(N_LESS_THAN_MAX));
                sum += N_LESS_THAN_MAX;
            }
            var tokensBefore = bucket.CurrentTokenCount;
            Assert.Equal(tokensBefore, (MAX_TOKENS - sum));

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(REFILL_INTERVAL);

            for (int i = 0; i < 3 * CUMULATIVE; i++)
            {
                bucket.ShouldThrottle(N_LESS_THAN_MAX);
            }

            var after = bucket.ShouldThrottle(N_LESS_THAN_MAX);
            var tokensAfter = bucket.CurrentTokenCount;

            Assert.True(after);
            Assert.True(tokensAfter < N_GREATER_THAN_MAX);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithCumulativeNGreaterThanMaxSleepCumulativeNLessThanMax()
        {
            SystemTime.SetCurrentTimeUtc = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = SystemTime.UtcNow;

            for (int i = 0; i < 3 * CUMULATIVE; i++)
                bucket.ShouldThrottle(N_LESS_THAN_MAX);

            var before = bucket.ShouldThrottle(N_LESS_THAN_MAX);
            var tokensBefore = bucket.CurrentTokenCount;

            Assert.True(before);
            Assert.True(tokensBefore < N_LESS_THAN_MAX);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(REFILL_INTERVAL);

            long sum = 0;
            for (int i = 0; i < CUMULATIVE; i++)
            {
                Assert.False(bucket.ShouldThrottle(N_LESS_THAN_MAX));
                sum += N_LESS_THAN_MAX;
            }

            var tokensAfter = bucket.CurrentTokenCount;
            Assert.Equal(tokensAfter, MAX_TOKENS - sum);
        }


        [Fact]
        public void ShouldThrottle_WhenCalledWithCumulativeNGreaterThanMaxSleepCumulativeNGreaterThanMax()
        {
            SystemTime.SetCurrentTimeUtc = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = SystemTime.UtcNow;

            for (int i = 0; i < 3 * CUMULATIVE; i++)
                bucket.ShouldThrottle(N_LESS_THAN_MAX);

            var before = bucket.ShouldThrottle(N_LESS_THAN_MAX);
            var tokensBefore = bucket.CurrentTokenCount;

            Assert.True(before);
            Assert.True(tokensBefore < N_LESS_THAN_MAX);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(REFILL_INTERVAL);

            for (int i = 0; i < 3 * CUMULATIVE; i++)
            {
                bucket.ShouldThrottle(N_LESS_THAN_MAX);
            }
            var after = bucket.ShouldThrottle(N_LESS_THAN_MAX);
            var tokensAfter = bucket.CurrentTokenCount;

            Assert.True(after);
            Assert.True(tokensAfter < (N_LESS_THAN_MAX));
        }

        [Fact]
        public void ShouldThrottle_WhenThread1NLessThanMaxAndThread2NLessThanMax()
        {
            var t1 = new Thread(p =>
            {
                var throttle = bucket.ShouldThrottle(N_LESS_THAN_MAX);
                Assert.False(throttle);
            });

            var t2 = new Thread(p =>
            {
                var throttle = bucket.ShouldThrottle(N_LESS_THAN_MAX);
                Assert.False(throttle);
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.Equal(bucket.CurrentTokenCount, (MAX_TOKENS - 2 * N_LESS_THAN_MAX));

        }

        [Fact]
        public void ShouldThrottle_Thread1NGreaterThanMaxAndThread2NGreaterThanMax()
        {
            var shouldThrottle = bucket.ShouldThrottle(N_GREATER_THAN_MAX);
            Assert.True(shouldThrottle);

            var t1 = new Thread(p =>
            {
                var throttle = bucket.ShouldThrottle(N_GREATER_THAN_MAX);
                Assert.True(throttle);
            });

            var t2 = new Thread(p =>
            {
                var throttle = bucket.ShouldThrottle(N_GREATER_THAN_MAX);
                Assert.True(throttle);
            });

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Assert.Equal(bucket.CurrentTokenCount, MAX_TOKENS);

        }
    }
}