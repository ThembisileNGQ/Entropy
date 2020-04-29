using System;

namespace Streams.Experiment.Domain
{
    public abstract class Command : AggregateMessage
    {
        protected Command(Guid id)
            : base(id)
        {
        }
    }

    public abstract class Query : AggregateMessage
    {
        protected Query(Guid id) 
            : base(id)
        {
        }
    }

    public abstract class AggregateMessage
    {
        public Guid Id { get; }

        public AggregateMessage(
            Guid id)
        {
            Id = id;
        }
    }
}