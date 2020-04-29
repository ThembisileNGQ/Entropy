using System;
using Akka.Actor;
using Akka.Event;
using Streams.Experiment.Domain.Aggregates;

namespace Streams.Experiment.Domain
{
    public class AggregateManager : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        public IActorRef ClubActorRef { get; }
        
        public AggregateManager()
        {
            ClubActorRef = Context.ActorOf(Props.Create(() => new Club()));
            _logger = Context.GetLogger();
            
            Receive<UserMessage>(Forward);
            Receive<BankAccountMessage>(Forward);
            Receive<AddMember>(Forward);
        }

        public bool Forward(AggregateMessage message)
        {
            _logger.Info("Received message of Type={0}", message.GetType());
            var aggregate = FindOrCreate(message);
            
            aggregate.Forward(message);
            return true;
        }

        public bool Forward(AddMember message)
        {
            ClubActorRef.Forward(message);
            
            return true;
        }
        
        protected virtual IActorRef FindOrCreate(AggregateMessage message)
        {
            var idToken = message switch
            {
                BankAccountMessage _ => "bank",
                UserMessage _ => "user",
                _ => null
            };
            var aggregateId = $"{idToken}-{message.Id}";
            var aggregate = Context.Child(aggregateId);

            if(aggregate.IsNobody())
            {
                Props props = message switch
                {
                    BankAccountMessage _ => Props.Create(() => new BankAccount()),
                    UserMessage _ => Props.Create(() => new User()),
                    _ => null
                };

                aggregate = CreateAggregate(props, aggregateId);
            }

            return aggregate;
        }
        
        protected virtual IActorRef CreateAggregate(Props props, string aggregateId)
        {
            var aggregateRef = Context.ActorOf(props, aggregateId);
            Context.Watch(aggregateRef);
            return aggregateRef;
        }
        
    }

    public class AggregateManagerRef
    {
        public IActorRef Ref { get; }

        public AggregateManagerRef(IActorRef aggregateManagerRef)
        {
            Ref = aggregateManagerRef;
        }
    }
}