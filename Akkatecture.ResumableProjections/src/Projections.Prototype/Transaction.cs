using System;
using System.Collections.Generic;
using Akka.Persistence.Query;

namespace Projections.Prototype
{
    public class Transaction
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Id { get; set; }
        public string StreamId { get; set; }
        public ICollection<EventEnvelope> Events { get; set; }
        public long Checkpoint { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        
        public Transaction()
        {
            Events = new List<EventEnvelope>();
            Checkpoint = -1L;
        }
    }
}