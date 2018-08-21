namespace Akka.Persistence.Performance.Actors.Commands
{
    public sealed class FinishCommand
    {
        public static FinishCommand Instance { get; } = new FinishCommand();
        public FinishCommand()
        {
            
        }
    }
}