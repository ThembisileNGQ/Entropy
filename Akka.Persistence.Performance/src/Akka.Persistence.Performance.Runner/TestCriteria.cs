using System;
using Akka.Configuration;

namespace Akka.Persistence.Performance.Runner
{
    public class TestCriteria
    {
        public Config Config { get; }
        public int ActorCount { get; }
        public int MessagesPerActor { get; }

        public TestCriteria(
            Config config,
            int actorCount,
            int messagesPerActor)
        {
            if(config == null)
                throw new ArgumentNullException($"{nameof(config)} can not be null.");
            
            if(actorCount < 1)
                throw new ArgumentException($"{nameof(actorCount)} needs to be positive.");
            
            if(messagesPerActor < 1)
                throw new ArgumentException($"{nameof(messagesPerActor)} needs to be positive.");

            Config = config.WithFallback(ConfigurationFactory.Default());
            ActorCount = actorCount;
            MessagesPerActor = messagesPerActor;

        }
    }
}