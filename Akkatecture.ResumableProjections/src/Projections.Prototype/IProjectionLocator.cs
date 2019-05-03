using Akkatecture.Aggregates;

namespace Projections.Prototype
{
    public interface IProjectionLocator<out TIdentity>
        where TIdentity : IProjectionId
    {
        TIdentity LocateProjector(IDomainEvent domainEvent);
    }
}