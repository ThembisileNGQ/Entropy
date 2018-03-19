using System.Threading;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Subscribers
{
    public interface ISubscribeSynchronousTo<TAggregate, in TIdentity, in TEvent>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        void Handle(IDomainEvent<TAggregate, TIdentity, TEvent> domainEvent);
    }
}