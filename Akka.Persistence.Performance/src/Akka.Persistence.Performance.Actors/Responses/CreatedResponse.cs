namespace Akka.Persistence.Performance.Actors.Responses
{
    public sealed class CreatedResponse
    {
        public static CreatedResponse Instance { get; } = new CreatedResponse();
        public CreatedResponse()
        {
            
        }
    }
}