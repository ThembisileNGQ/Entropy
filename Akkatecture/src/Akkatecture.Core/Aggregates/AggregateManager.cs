using Akka.Actor;
using Akka.Event;
using Akkatecture.Commands;
using Akkatecture.Core;
using Akkatecture.Extensions;
using Akkatecture.Messaging;

namespace Akkatecture.Aggregates
{
    public abstract class AggregateManager<TAggregate, TIdentity, TCommand, TState> : ReceiveActor
        where TAggregate : AggregateRoot<TAggregate, TIdentity, TState>
        where TState : AggregateState<TAggregate, TIdentity, IEventApplier<TAggregate, TIdentity>>
        where TIdentity : IIdentity
        where TCommand : ICommand<TAggregate,TIdentity>
    {
        private ILoggingAdapter Logger { get; set; }

        protected AggregateManager()
        {
            Logger = Context.GetLogger();

            Become(Manager);
        }

        protected virtual void Manager()
        {
            Receive<Envelope<TAggregate, TIdentity, TCommand>>(Dispatch);
            Receive<object>(Fail);
        }

        protected virtual bool Dispatch(Envelope<TAggregate, TIdentity, TCommand> message)
        {
            Logger.Info($"{this.GetType().DeclaringType} received {message.GetType()}");
            
            var aggregateRef = FindOrCreate(message.Command.AggregateId);

            aggregateRef.Tell(message.Command);

            return true;
        }

        protected virtual bool Fail(object message)
        {
            Logger.Warning($"{this.GetType().DeclaringType} received {message.GetType()}");

            return true;
        }

        protected virtual IActorRef FindOrCreate(TIdentity aggregateId)
        {
            var aggregateRef = Context.Child(aggregateId);

            if (Equals(aggregateRef,ActorRefs.Nobody))
            {
                aggregateRef = CreateAggregate(aggregateId);
            }

            Context.Watch(aggregateRef);

            return aggregateRef;
        }

        protected virtual IActorRef CreateAggregate(TIdentity aggregateId)
        {
            var aggregateRef = Context.ActorOf(Props.Create<TAggregate>(aggregateId),aggregateId.Value);

            return aggregateRef;
        }
    }
}