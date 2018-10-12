# Entropy
Brain Dump of silly ideas. A bunch of POCs and code sketches & WIPS. 

## Akka.AsyncPerformance
A little bit of code that shows how two actors doing the same thing result in different throughputs. async/await vs PipeTo with some astonishing test results that you can reproduce. [go here to find out more...](https://github.com/Lutando/Entropy/tree/master/Akka.AsyncPerformance)

## Akka.Hosting
This is a silly thing that shows how to host akka.net applications in the future.
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/Akka.Hosting)

## Akka.Persistence.Performance
This is a silly thing that ive written to test out persistence plugins and how they fair under stress. You could use this to see how far you could bend your persistence store to be honest. Its a n outrageous test because the results are pretty uncontroversial
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/Akka.Persistence.Performance)

## Akka.Websockets
Currently a WIP on best practices for AspNetCore Websockets and ActorSystem integration with Akka.net.
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/Akka.Websockets)

## ElasticK8s.Api
A small thing that shows how one can deploy to k8s and scale your deployment from within the cluster. Follow the readme from within the project to see how this is possible. Only tested from a minikube cluster.
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/ElasticK8s.Api)

## ElasticK8s.Prototype
Was a precurser to the `ElasticK8s.Api` experiment. Wanted to see how the k8s client API looked like.
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/ElasticK8s.Prototype)

## HAL.Prototype
In the persuit of understanding HATEOAS, I wanted to see how one such API might look like.
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/HAL.Prototype)

## Transcoding.Prototype
Im tryna transcode audio using actors that wrap around ffmpeg, it is silly, move on.
[go here to find out more...](https://github.com/Lutando/Entropy/tree/master/Transcoding.Prototype)
