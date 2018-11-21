using Akkatecture.Aggregates;
using Domain.Model.Car.Events;

namespace Domain.Model.Car
{
    public class CarState: AggregateState<CarAggregate,CarId>,
        IApply<CarCreatedEvent>,
        IApply<CarNameChangedEvent>
    {
        public string Name { get; private set; }
        
        public void Apply(CarCreatedEvent aggregateEvent)
        {
            
        }

        public void Apply(CarNameChangedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
        }
    }
}