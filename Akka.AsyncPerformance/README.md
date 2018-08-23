# Akka.AsyncPerformance
Testing out the real difference between async await in actors and pipeto

# Setup
2 Actors were implemented, each were delayed using `Task.Delay(n)` to simiulate an asynchronous task.
 The `AwaitAsyncActor` simply awaited on the delay to increment an internal state counter, while the `PipeToActor` piped a post-delay operation to the actor to increment its internal state counter, both actors implement a simple integer variable to hold this state.

## Results

```
Test Scenario 1
Async Delay = 10ms
Number Of Messages = 10

asyncAwaitActor
Time : 155 ms.
Test Pass : True


PipeToActor
Time : 14 ms.
Test Pass : True


Test Scenario 1
Async Delay = 50ms
Number Of Messages = 10

asyncAwaitActor
Time : 572 ms.
Test Pass : True


PipeToActor
Time : 57 ms.
Test Pass : True

Test Scenario 2
Async Delay = 50ms
Number Of Messages = 10

asyncAwaitActor
Time : 1209 ms.
Test Pass : True


PipeToActor
Time : 13 ms.
Test Pass : True

Test Scenario 3
Async Delay = 10ms
Number Of Messages = 100

asyncAwaitActor
Time : 1074 ms.
Test Pass : True


PipeToActor
Time : 107 ms.
Test Pass : True

Test Scenario 4
Async Delay = 100ms
Number Of Messages = 100

asyncAwaitActor
Time : 10324 ms.
Test Pass : True

PipeToActor
Time : 107 ms.
Test Pass : True


```