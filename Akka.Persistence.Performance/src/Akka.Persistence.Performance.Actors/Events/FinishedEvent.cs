namespace Akka.Persistence.Performance.Actors.Events
{
    public sealed class FinishedEvent
    {
        public long State { get; }

        public FinishedEvent(long state)
        {
            State = state;
        }
    }
}