# Throttling API [![Build Status](https://travis-ci.org/sudohippie/throttle.png)](https://travis-ci.org/sudohippie/throttle)

## Overview
API to throttle/rate-limit requests

This API implements two popular throttling strategies, namely:

1. Fixed token bucket
2. Leaky token bucket

### Fixed token bucket
Details for this implementation can be found at: [Token Bucket](http://en.wikipedia.org/wiki/Token_bucket) 

### Leaky token bucket
Details for this implementation can be found at: [Leaky Bucket](http://en.wikipedia.org/wiki/Leaky_bucket)
With in the API, Leaky buckets have been implemented as two types

1. StepDownLeakyTokenBucketStrategy
2. StepUpLeakyTokenBucketStrategy

StepDownLeakyTokenBucketStrategy resembles a bucket which has been filled with tokens at the beginning but subsequently leaks tokens at a fixed interval.
StepUpLeakyTokenBucketStrategy resemembles an empty bucket at the beginning but get filled will tokens over a fixed interval.

## Examples

### Fixed Bucket Example

```java
// construct strategy
ThrottleStrategy strategy = new FixedTokenBucketStrategy(10, 1, 1000);

// provide the strategy to the throttler
Throttle throttle = new Throttle(strategy);

// throttle :)
boolean isThrottled = throttle.CanConsume();

if(!isThrottled){ 
  // your logic
}
```

### Step Up Leaky Bucket Example
```java
// construct strategy
ThrottleStrategy strategy = new StepUpLeakyTokenBucket(100, 1, 1000, 25, 15, 1000);

// provide the strategy to the throttler
Throttle throttle = new Throttle(strategy);

// throttle :)
boolean isThrottled = throttle.CanConsume();

if(!isThrottled){ 
  // your logic
}
```

### Step Down Leaky Bucket Example
```java
// construct strategy
ThrottleStrategy strategy = new StepDownLeakyTokenBucket(100, 1, 1000, 25, 15, 1000);

// provide the strategy to the throttler
Throttle throttle = new Throttle(strategy);

// throttle :)
boolean isThrottled = throttle.CanConsume();

if(!isThrottled){ 
  // your logic
}
```

### Wait For Token Availability Example
```java
// construct strategy
ThrottleStrategy strategy = new FixedTokenBucketStrategy(100, 1, 1000);

// provide the strategy to the throttler
Throttle throttle = new Throttle(strategy);

while(!throttle.canProceed()){
    Thread.sleep(throttle.waitTime(TimeUnit.MILLISECONDS));
}

// your logic
```

