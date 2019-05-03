﻿using System;
using System.Collections.Generic;

namespace Projections.Prototype
{
    public class ProjectionContext
    {
        public string TransactionId { get; set; }
        public string StreamId { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public long Checkpoint { get; set; }
        public IDictionary<string, object> EventHeaders { get; set; }
        public IDictionary<string, object> TransactionHeaders { get; set; }
    }
}