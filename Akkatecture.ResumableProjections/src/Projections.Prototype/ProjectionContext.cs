using System;
using System.Collections.Generic;

namespace Projections.Prototype
{
    public class ProjectionContext
    {
        public string TransactionId { get; set; }
        public string StreamId { get; set; }
        public long Checkpoint { get; set; }
        public DateTimeOffset StartTimestamp { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public IDictionary<string, object> EventHeaders { get; set; }
        public IDictionary<string, object> TransactionHeaders { get; set; }
    }
}
