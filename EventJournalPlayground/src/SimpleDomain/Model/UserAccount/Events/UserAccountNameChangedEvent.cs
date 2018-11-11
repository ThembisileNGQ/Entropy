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
    
    [EventVersion("UserAccountNameChanged", 2)]
    public class UserAccountNameChangedEventV2 : AggregateEvent<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountNameChangedEventV2(string name)
        {
            Name = name + "but v2 tho";
        }
    }
}