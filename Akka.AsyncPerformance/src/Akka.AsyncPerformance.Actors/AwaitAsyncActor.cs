using System;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;

namespace Akka.AsyncPerformance.Actors
{
    public class AwaitAsyncActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private readonly int _delay;
        public int Store { get; private set; }
        
        public AwaitAsyncActor()
        {
            Store = 0;
            _logger = Context.GetLogger();
            _delay = Context.System.Settings.Config.GetInt("test.delay");
            ReceiveAsync<AddOneCommand>(Handle);
            Receive<GetResultsQuery>(Handle);
        }

        public async Task Handle(AddOneCommand command)
        {
            //_logger.Info($"processing [{command.MessageId}]");
            
            await Task.Delay(TimeSpan.FromMilliseconds(_delay));

            Store = Store + 1;
            
            //_logger.Info($"async await actor finished [{command.MessageId}]");
            
        }

        public bool Handle(GetResultsQuery query)
        {
            var result = new StateResult(Store);
            Sender.Tell(result);
            return true;
        }
    }
}