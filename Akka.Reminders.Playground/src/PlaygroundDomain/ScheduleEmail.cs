using System;

namespace PlaygroundDomain
{
    public class ScheduleEmail
    {
        public string From { get; }
        public string To { get; }
        public string Body { get; }

        public ScheduleEmail(
            string from,
            string to,
            string body)
        {
            From = from;
            To = to;
            Body = body;
        }
    }

    public class Ack
    {
        public string Id { get; private set; }

        public Ack(string id)
        {
            Id = id;
        }
    }
}