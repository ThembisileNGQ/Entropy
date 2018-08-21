using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Pattern;
using Akka.Persistence.Performance.Actors;
using Akka.Persistence.Performance.Actors.Commands;
using Akka.Persistence.Performance.Actors.Responses;

namespace Akka.Persistence.Performance.Runner
{
    public sealed class TestRunner
    {
        private TestCriteria Criteria { get; }
        private string JournalPlugin { get; }
        private TestRunner(TestCriteria criteria)
        {
            if(criteria == null)
                throw new ArgumentNullException($"{nameof(criteria)} cannot be null");
            
            Criteria = criteria;
            JournalPlugin = Criteria.Config.GetString("akka.persistence.journal.plugin");
        }
        public static TestRunner Init(TestCriteria criteria)
        {
            return new TestRunner(criteria);
        }

        public TestResult Run()
        {
            Console.WriteLine("Performance benchmark starting.\n");
            Console.WriteLine($"for journal plugin {JournalPlugin}\n");

            var actorSystemId = Guid.NewGuid();
            using (var system = ActorSystem.Create($"persistent-benchmark-{actorSystemId}",Criteria.Config))
            {
                var actors = new IActorRef[Criteria.ActorCount];
                for (int i = 0; i < Criteria.ActorCount; i++)
                {
                    var pid = actorSystemId + "-" + i;
                    actors[i] = system.ActorOf(Props.Create(() => new PerformanceActor(pid)));
                }
                
                Task.WaitAll(
                    actors
                        .Select(a => a.Ask<CreatedResponse>(CreateCommand.Instance))
                        .Cast<Task>()
                        .ToArray()
                    );
                
                Console.WriteLine($"All {Criteria.ActorCount} actors have been initialized.");
                
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                
                for (int i = 0; i < Criteria.MessagesPerActor; i++)
                {
                    for (int j = 0; j < Criteria.ActorCount; j++)
                    {
                        actors[j].Tell(new StoreValueCommand(1));
                    }
                }
                
                var finished = new Task[Criteria.ActorCount];
                for (int i = 0; i < Criteria.ActorCount; i++)
                {
                    finished[i] = actors[i].Ask<StateResponse>(FinishCommand.Instance);
                }
                
                Task.WaitAll(finished);

                var elapsed = stopwatch.ElapsedMilliseconds;
                
                foreach (Task<StateResponse> task in finished)
                {
                    if (!task.IsCompleted || task.Result.State != Criteria.MessagesPerActor)
                        throw new IllegalStateException("Actor's state was invalid");
                }
                
                return TestResult.From(Criteria,elapsed);
            }
        }
    }
}