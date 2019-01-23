using Akkatecture.Aggregates;

namespace Domain.Model.Car.Events
{
    public class CarCreatedEvent : AggregateEvent<CarAggregate, CarId>
    {
    }
}