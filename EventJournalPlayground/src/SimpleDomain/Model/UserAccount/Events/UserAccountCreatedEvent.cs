using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace SimpleDomain.Model.UserAccount.Events
{
    [EventVersion("UserAccountCreated", 1)]
    public class UserAccountCreatedEvent : AggregateEvent<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountCreatedEvent(string name)
        {
            Name = name;
        }
    }
    
}
