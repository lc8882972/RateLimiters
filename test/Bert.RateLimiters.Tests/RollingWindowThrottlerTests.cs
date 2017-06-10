using System;
using Xunit;

namespace Bert.RateLimiters.Tests
{
    public class RollingWindowThrottlerTests
    {
        private readonly DateTime referenceTime = new DateTime(2014, 9, 20, 0, 0, 0, DateTimeKind.Utc);
       
        [Fact]
        public void Throws_WhenNumberOfOccurencesIsLesserThanOne()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new RollingWindowThrottler(-1, TimeSpan.FromSeconds(5)));
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithTokensLessThanOne_WillThrow()
        {
            var throttler = new RollingWindowThrottler(1, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            Assert.Throws<ArgumentOutOfRangeException>(() => throttler.ShouldThrottle(0, out waitTimeMillis));
        }

        [Fact]
        public void ShouldThrottle_WhenCalled_WillReturnFalse()
        {
            var throttler = new RollingWindowThrottler(1, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);

            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
            
        }

        [Fact]
        public void ShouldThrottle_WhenCalledTwiceinSameSecondAndAllows1PerSecond_WillReturnTrue()
        {

            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(1, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(0.5);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeTrue();
            Assert.True(shouldThrottle);

            //waitTimeMillis.ShouldEqual(500);
            Assert.Equal(waitTimeMillis, 500);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(0.8);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeTrue();
            Assert.True(shouldThrottle);

            //waitTimeMillis.ShouldEqual(200);
            Assert.Equal(waitTimeMillis, 200);
        }


        [Fact]
        public void ShouldThrottle_WhenCalledAfterSecondPassAndAllows1PerSecond_WillReturnFalse()
        {

            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(1, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(1);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledTwiceinSameSecondAndAllows2PerSecond_WillReturnFalse()
        {

            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(2, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(0.5);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledAfterSecondPassesAndAllows2PerSecond_WillReturnFalse()
        {

            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(2, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(1);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
            //waitTimeMillis.ShouldEqual(0);
            Assert.Equal(waitTimeMillis,0);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledThreeTimesinSameSecondAndAllows2PerSecond_WillReturnTrue()
        {

            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(2, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(0.5);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(0.7);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeTrue();
            Assert.True(shouldThrottle);
            //waitTimeMillis.ShouldEqual(300);
            Assert.Equal(waitTimeMillis, 300);
        }


        [Fact]
        public void ShouldThrottle_WhenCalledAtEndOfRollingWindowAndAllows2PerSecond_WillReturnFalse()
        {

            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(2, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            //first rolling window expired, init a new one
            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(1.2);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            //inside second rolling window, under threshold
            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(1.3);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            //second rolling window expired, beginning third window
            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(2.2);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            //third window, under threshold
            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(2.3);
            shouldThrottle = throttler.ShouldThrottle(1, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithMoreTokensThanOccurrences_WillReturnTrue()
        {
            var throttler = new RollingWindowThrottler(2, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(3, out waitTimeMillis);
            //shouldThrottle.ShouldBeTrue();
            Assert.True(shouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledAndConsumingAllTokensAtOnce_WillReturnFalse()
        {
            var throttler = new RollingWindowThrottler(3, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(3, out waitTimeMillis);

            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledAndConsumingAllTokensAtOnceAndThenCalledOnceMore_WillReturnTrue()
        {
            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(3, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(3, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(0.2);
            shouldThrottle = throttler.ShouldThrottle(3, out waitTimeMillis);
            //shouldThrottle.ShouldBeTrue()
            Assert.True(shouldThrottle);
            //waitTimeMillis.ShouldEqual(800);
            Assert.Equal(waitTimeMillis,800);
        }


        [Fact]
        public void ShouldThrottle_WhenCalledAndConsumingAllTokensAtOnceAndThenCalledOnceMoreAfterRollingWindowEnd_WillReturnFalse()
        {
            SystemTime.SetCurrentTimeUtc = () => referenceTime;
            var virtualNow = SystemTime.UtcNow;

            var throttler = new RollingWindowThrottler(3, TimeSpan.FromSeconds(1));
            long waitTimeMillis;
            var shouldThrottle = throttler.ShouldThrottle(3, out waitTimeMillis);
            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            SystemTime.SetCurrentTimeUtc = () => virtualNow.AddSeconds(1.1);
            shouldThrottle = throttler.ShouldThrottle(3, out waitTimeMillis);

            //shouldThrottle.ShouldBeFalse();
            Assert.False(shouldThrottle);

            //waitTimeMillis.ShouldEqual(0);
            Assert.Equal(waitTimeMillis,0);
        }

    }
}