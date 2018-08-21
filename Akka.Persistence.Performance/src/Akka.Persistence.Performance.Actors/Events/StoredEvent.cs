namespace Akka.Persistence.Performance.Actors.Events
{
    public sealed class StoredEvent
    {
        public int Value { get; }

        public StoredEvent(int value)
        {
            Value = value;
        }
    }
}