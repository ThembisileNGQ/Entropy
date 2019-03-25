using LaundryBooker.Domain.Base.Identity;

namespace LaundryBooker.Domain.Base.Entity
{
    public interface IEntity
    {
        IIdentity GetIdentity();
    }

    public interface IEntity<out TIdentity> : IEntity
        where TIdentity : IIdentity
    {
        TIdentity Id { get; }
    }
}