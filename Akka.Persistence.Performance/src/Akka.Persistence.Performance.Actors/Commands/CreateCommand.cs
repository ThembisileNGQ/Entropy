using System;

namespace Akka.Persistence.Performance.Actors.Commands
{
    public sealed class CreateCommand
    {
        public static CreateCommand Instance { get; } = new CreateCommand();
        public CreateCommand()
        {
        }
    }
}