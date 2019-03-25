using LaundryBooker.Domain.Base.Entity;
using LaundryBooker.Domain.Base.Identity;

namespace LaundryBooker.Domain.Base.AggregateRoot
{
    public abstract class AggregateRoot<TIdentity> : Entity<TIdentity>
        where TIdentity : IIdentity
    {
        protected AggregateRoot(TIdentity id)
            : base(id)
        {
        }
    }
}