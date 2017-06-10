using Bert.RateLimiters;
using System;

namespace Hello
{
    class Program
    {
        static void Main(string[] args)
        {
            IThrottleStrategy strategy = new FixedTokenBucket(100, 10, 1000);
            Throttler throttle = new Throttler(strategy);
            
            if (!throttle.CanConsume())
            {
                Console.WriteLine("CanConsume");
            }
            Console.WriteLine("Hello World!");
        }
    }
}