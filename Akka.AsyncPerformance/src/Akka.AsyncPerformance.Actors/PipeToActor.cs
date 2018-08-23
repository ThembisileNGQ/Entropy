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
        public int Store { get; private set; }
        public int MaxStore { get; private set; }

        public PipeToActor(int maxStore)
        {
            Store = 0;
            MaxStore = maxStore;
            _delay = Context.System.Settings.Config.GetInt("test.delay");
            _logger = Context.GetLogger();
            
            Receive<AddOneCommand>(Handle);
            Receive<GetResultsQuery>(Handle);
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
            _logger.Info($"pipe to actor finished [{command.MessageId}]");
            Store++;
            
            
            return true;
        }

        public bool Handle(GetResultsQuery query)
        {
            if (Store < MaxStore)
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