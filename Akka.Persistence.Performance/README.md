# Akka Peristence Performance Testing Example

Little program that gives arbitrary performance values based on machine and backing persistence store

Edit the configuration in the `Akka.Persistence.Performance.Runner` Program.cs Main method to begin the test, and set the relevant test criteria as well

## Experiment
The experiment brute forces through the akka persistence engine by making actors add 1 to their internal state since we are using `EventSourced.Persist` to persist our events, we guarantee the processing of the next command only one persist has been completed. Feel free to change the `TestCriteria`values that can influence the significance and length of your tests.

## Results
Keep in mind these results are using the latest persistence plugins for the backing stores mentioned. Another thing that can influence the persistence plugin is the serializer which ends up dictating how the bits get pushed down the wire (and how long it takes to serialize/deserialize the messaes). In memory plugin is expected to be the fastest since it doesnt have to serialize in proc and the IO is all done on the same machine.

```
INMEMORY

1000 actors stored 100 events each in 3.269 sec. Average: 30590.3946160905 events/sec

REDIS (PROTOBUF)

1000 actors stored 100 events each in 30.866 sec. Average: 3239.81079504957 events/sec

REDIS (MSGPACK)

1000 actors stored 100 events each in 26.924 sec. Average: 3714.15837171297 events/sec

POSTGRES (BYTEA)

1000 actors stored 100 events each in 184.543 sec. Average: 541.87912844161 events/sec

POSTGRES JSONB

1000 actors stored 100 events each in 195.04 sec. Average: 512.715340442986 events/sec
```

I cant not really explain why MSGPACK is "faster" than PROTOBUF in redis but for now ill leave this here for discussion 🤷, may be just implementation detail.

## Conclusion

Inmemory is an order of magnitude faster than redis,
Redis is an order of magnitude faster than postgres

But you knew that already.

## Test Rig

Tests run on
```
i7 7th Generation Intel Processor
16GB ram
500GB SSD 
backing stores were run in virtual envioronments
(k8s in minikube running dockerized redis and postgres pods)
```

Future considerations - add these test cases as configured so it can encourage re-runability for others to experiment with.