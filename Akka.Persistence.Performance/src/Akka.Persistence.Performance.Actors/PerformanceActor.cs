using Akka.Persistence.Performance.Actors.Commands;
using Akka.Persistence.Performance.Actors.Events;
using Akka.Persistence.Performance.Actors.Responses;

namespace Akka.Persistence.Performance.Actors
{
    public class PerformanceActor : ReceivePersistentActor
    {
        public override string PersistenceId { get; }
        private long State { get; set; }

        public PerformanceActor(string persistenceId)
        {
            PersistenceId = persistenceId;
            State = 0L;
            
            Command<CreateCommand>(Handle);
            Command<StoreValueCommand>(Handle);
            Command<FinishCommand>(Handle);

            Recover<StoredEvent>(Recover);
        }

        private bool Handle(CreateCommand command)
        {
            var evt = new StoredEvent(0);
            Persist(evt, x =>
            {
                Apply(x);
                var response = CreatedResponse.Instance;
                Sender.Tell(response, Self);
            });
            return true;
        }
        
        private bool Handle(StoreValueCommand command)
        {
            var evt = new StoredEvent(command.Value);
            Persist(evt,Apply);
            return true;
        }

        private bool Handle(FinishCommand command)
        {
            var response = new StateResponse(State);
            Sender.Tell(response, Self);
            return true;
        }

        private bool Recover(StoredEvent evt)
        {
            Apply(evt);
            return true;
        }
        private void Apply(StoredEvent evt)
        {
            State += evt.Value;
        }
    }
}