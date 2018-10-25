using Akkatecture.Aggregates;

namespace Domain.Model.Car.Events
{
    public class CarNameChangedEvent : AggregateEvent<CarAggregate, CarId>
    {
        public string Name { get; }
        public CarNameChangedEvent(string name)
        {
            Name = name;
        }
    }
}