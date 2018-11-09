using Akka;
using Akka.Persistence.Journal;
using Akkatecture.Aggregates;
using SimpleDomain.Model.UserAccount;
using SimpleDomain.Model.UserAccount.Events;

namespace SimpleDomain
{
    public class SimpleEventTagger : IWriteEventAdapter
    {
        public string Manifest(object evt) => string.Empty;

        public object ToJournal(object evt)
        {
            switch (evt)
            {
                case CommittedEvent<UserAccountAggregate, UserAccountId, UserAccountCreatedEvent> s:
                    return new Tagged(evt,new[] {"UserAccountAggregate.UserAccountCreatedEvent.0"});
                
                case CommittedEvent<UserAccountAggregate, UserAccountId, UserAccountNameChangedEvent> s:
                    return new Tagged(evt,new[] {"UserAccountAggregate.UserAccountNameChangedEvent.0"});
            }

            return evt;

        }
        
        
        
    }
}