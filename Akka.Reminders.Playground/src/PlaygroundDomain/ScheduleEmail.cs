using System;

namespace PlaygroundDomain
{
    public class ScheduleEmail
    {
        public string From { get; }
        public string To { get; }
        public string Body { get; }
        public DateTime DueAfter { get; }

        public ScheduleEmail(
            string from,
            string to,
            string body,
            DateTime dueAfter)
        {
            From = from;
            To = to;
            Body = body;
            DueAfter = dueAfter;
        }
    }
}