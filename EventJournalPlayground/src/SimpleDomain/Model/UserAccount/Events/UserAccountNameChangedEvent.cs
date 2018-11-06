using Akkatecture.Aggregates;
using Akkatecture.Events;

namespace SimpleDomain.Model.UserAccount.Events
{
    [EventVersion("UserAccountNameChanged", 1)]
    public class UserAccountNameChangedEvent : AggregateEvent<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountNameChangedEvent(string name)
        {
            Name = name;
        }
    }
}