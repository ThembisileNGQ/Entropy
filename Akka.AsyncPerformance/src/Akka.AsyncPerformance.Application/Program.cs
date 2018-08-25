using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.AsyncPerformance.Actors;
using Akka.Configuration;

namespace Akka.AsyncPerformance.Application
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var numberOfMessages = 10;
            var delay = 100;
            var configString = $@"test.delay = {delay}";
            var config = ConfigurationFactory.ParseString(configString);
            
            var asyncAwaitActorSystem = ActorSystem.Create("async-await-system", config);
            var pipeToactorSystem = ActorSystem.Create("pipe-to-system", config);

            var asyncAwaitActor = asyncAwaitActorSystem.ActorOf(Props.Create(() => new AwaitAsyncActor()));

            var pipeToActor = pipeToactorSystem.ActorOf(Props.Create(() => new PipeToActor(numberOfMessages)));

            var asyncAwaitResult = await Trial(asyncAwaitActor, numberOfMessages);
            var pipeToResult = await Trial(pipeToActor, numberOfMessages);
            
            Console.WriteLine("asyncAwaitActor");
            Console.WriteLine(asyncAwaitResult);
            Console.WriteLine("\n");
            Console.WriteLine("PipeToActor");
            Console.WriteLine(pipeToResult);
        }

        public static async Task<TrialResult> Trial(IActorRef actor, int numberOfMessages)
        {
            var asyncWatch = new Stopwatch();
            asyncWatch.Start();
            
            for (int i = 0; i < numberOfMessages; i++)
            {
                var command = new AddOneCommand(i);
                actor.Tell(command);
            }
            
            var result = new GetFinalResultsQuery();
            var state = await actor.Ask<StateResult>(result);
            asyncWatch.Stop();
            
            return new TrialResult(state.Value,numberOfMessages,asyncWatch.ElapsedMilliseconds);
        }
        
       
    }
}