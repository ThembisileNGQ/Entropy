namespace Akka.Persistence.Performance.Actors.Commands
{
    public sealed class StoreValueCommand
    {
        public int Value { get; }

        public StoreValueCommand(int value)
        {
            Value = value;
        }
    }
}