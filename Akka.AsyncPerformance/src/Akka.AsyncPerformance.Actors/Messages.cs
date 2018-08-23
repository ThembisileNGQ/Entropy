namespace Akka.AsyncPerformance.Actors
{
    public class AddOneCommand
    {
        public int MessageId { get;}
        public AddOneCommand(int messageId)
        {
            MessageId = messageId;
        }
    }
    
    public class PipeAddOneCommand
    {
        public int MessageId { get;}
        public PipeAddOneCommand(int messageId)
        {
            MessageId = messageId;
        }
    }

    public class GetFinalResultsQuery
    {
        public GetFinalResultsQuery()
        {
            
        }
    }

    public class StateResult
    {
        public int Value { get; }
        
        public StateResult(int value)
        {
            Value = value;
        }
    }
}