# Akka.AsyncPerformance
Testing out the real difference between async await in actors and pipeto by simulating asynchronous delay using Task.Delay().

# Setup
2 Actors were implemented, each were delayed using `Task.Delay(n)` to simiulate an asynchronous task.
 The `AwaitAsyncActor` simply awaited on the delay to increment an internal state counter, while the `PipeToActor` piped a post-delay operation to the actor to increment its internal state counter, both actors implement a simple integer variable to hold this state.

## Results

| Delay        | 10 ms  | 50 ms  | 10 ms   | 100 ms  | 100 ms   | 1000 ms    |
|--------------|--------|--------|---------|---------|----------|------------|
| **Messages** | **10** | **10** | **100** | **10**  | **100**  | **1000**   |
| AsyncAwait   | 155 ms | 572 ms | 1209 ms | 1074 ms | 10324 ms | 1003028 ms |
| PipeTo       | 15 ms  | 57     | 13 ms   | 107ms   | 107 ms   | 1010 ms    |

# Conclusion

The results are quite conclusive showing that the amount of time it takes to process `N` messages that expirience `D` delay per processing for PipeTo is `~ D ms` (the best we can hope for), and the amount of time it takes for AsyncAwait to process N messages that expirience D delay is `~ N x D ms` (the worst we could wish for).

## Test Rig

Tests run on
```
OSX
i7 MacBook Pro (2015)
16GB ram
500GB MAC FLASH
Akka 1.3.8
netcoreapp2.1
```