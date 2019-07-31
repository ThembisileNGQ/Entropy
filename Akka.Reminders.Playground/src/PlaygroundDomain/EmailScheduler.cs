using Akka.Persistence.Reminders;

namespace PlaygroundDomain
{
    public class EmailScheduler : Reminder
    {
        public EmailScheduler(ReminderSettings settings)
            : base(settings)
        {
            
        }
    }
}