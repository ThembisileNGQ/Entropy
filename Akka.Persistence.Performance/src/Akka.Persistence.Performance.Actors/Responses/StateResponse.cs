namespace Akka.Persistence.Performance.Actors.Responses
{
    public sealed  class StateResponse
    {
        public long State { get; }
        
        public StateResponse(long state)
        {
            State = state;
        }
    }
}