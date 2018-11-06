using Akkatecture.Aggregates;
using SimpleDomain.Model.UserAccount.Events;

namespace SimpleDomain.Model.UserAccount
{
    public class UserAccountState : AggregateState<UserAccountAggregate,UserAccountId>,
        IApply<UserAccountCreatedEvent>,
        IApply<UserAccountNameChangedEvent>
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

    }
}