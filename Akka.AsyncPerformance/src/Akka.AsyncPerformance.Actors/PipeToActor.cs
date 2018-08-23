using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace Akka.AsyncPerformance.Actors
{
    public class PipeToActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private readonly int _delay;
        private readonly int _maxStore;
        public int Store { get; private set; }
        
        public PipeToActor(int maxStore)
        {
            _maxStore = maxStore;
            _delay = Context.System.Settings.Config.GetInt("test.delay");
            _logger = Context.GetLogger();
            
            Store = 0;
            
            Receive<AddOneCommand>(Handle);
            Receive<GetFinalResultsQuery>(Handle);
            Receive<PipeAddOneCommand>(Handle);
        }

        public bool Handle(AddOneCommand command)
        {
           // _logger.Info($"processing [{command.MessageId}]");
            
            Task.Delay(TimeSpan.FromMilliseconds(_delay))
                .ContinueWith(continuation =>
                {
                    var cmd = command;
                    return new PipeAddOneCommand(cmd.MessageId);
                }, TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(Self);
            
            return true;
        }
        
        public bool Handle(PipeAddOneCommand command)
        {
            Store++;
            
            //_logger.Info($"pipe to actor finished [{command.MessageId}]");
            return true;
        }

        public bool Handle(GetFinalResultsQuery query)
        {
            if (Store < _maxStore)
            {
                Self.Tell(query,Sender);
            }
            else
            {
                var result = new StateResult(Store);
                Sender.Tell(result);
            }
            
            return true;
        }
    }
}