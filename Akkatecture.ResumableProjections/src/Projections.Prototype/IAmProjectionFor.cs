using Akkatecture.Aggregates;
using IIdentity = Akkatecture.Core.IIdentity;

namespace Projections.Prototype
{
    public interface IAmProjectionFor<TAggregate, in TIdentity, in TEvent>
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
        where TEvent : IAggregateEvent<TAggregate, TIdentity>
    {
        void Apply(IProjectionContext context,IDomainEvent<TAggregate, TIdentity, TEvent> domainEvent);
    }
}
