using System.Security.Cryptography;

namespace Akka.Persistence.Performance.Runner
{
    public class TestResult
    {
        public int ActorCount { get; }
        public int MessagesPerActor { get; }
        public long ElapsedMilliseconds { get; }
        public double ElapsedSeconds => ElapsedMilliseconds / 1000;
        public double Throughput => ActorCount * MessagesPerActor * 1000.0 / ElapsedMilliseconds;

        private TestResult(
            int actorCount,
            int messagesPerActor,
            long elapsedMilliseconds)
        {
            ActorCount = actorCount;
            MessagesPerActor = messagesPerActor;
            ElapsedMilliseconds = elapsedMilliseconds;
        }
        public static TestResult From(TestCriteria criteria, long elapsedMilliseconds)
        {
            return new TestResult(criteria.ActorCount,criteria.MessagesPerActor,elapsedMilliseconds);
        }

        public override string ToString()
        {
            return
                $"{ActorCount} actors stored {MessagesPerActor} events each in {ElapsedSeconds} seconds. Average: {Throughput} events/sec.";
        }
    }
}