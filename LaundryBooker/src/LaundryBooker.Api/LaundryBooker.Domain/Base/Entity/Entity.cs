using System;
using System.Collections.Generic;
using LaundryBooker.Domain.Base.Identity;
using LaundryBooker.Domain.Base.ValueObjects;

namespace LaundryBooker.Domain.Base.Entity
{
    public abstract class Entity<TIdentity> : ValueObject, IEntity<TIdentity>
        where TIdentity : IIdentity
    {
        protected Entity(TIdentity id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            Id = id;
        }

        public TIdentity Id { get; }

        public IIdentity GetIdentity()
        {
            return Id;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
        }
    }
}