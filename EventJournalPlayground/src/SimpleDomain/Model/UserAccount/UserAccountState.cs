using System.Runtime.InteropServices.ComTypes;
using Akkatecture.Aggregates;
using SimpleDomain.Model.UserAccount.Events;

namespace SimpleDomain.Model.UserAccount
{
    public class UserAccountState : AggregateState<UserAccountAggregate,UserAccountId>,
        IApply<UserAccountCreatedEvent>,
        IApply<UserAccountNameChangedEvent>,
        IApply<UserAccountNameChangedEventV2>
    {
        public string Name { get; private set; }

        public void Apply(UserAccountCreatedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
        }

        public void Apply(UserAccountNameChangedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
        }
        
        public void Apply(UserAccountNameChangedEventV2 aggregateEvent)
        {
            Name = aggregateEvent.Name + "new apply";
        }

    }
}