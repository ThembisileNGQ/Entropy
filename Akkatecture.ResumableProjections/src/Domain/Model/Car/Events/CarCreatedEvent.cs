using Akkatecture.Aggregates;

namespace Domain.Model.Car.Events
{
    public class CarCreated : AggregateEvent<CarAggregate, CarId>
    {
        public CarCreated()
        {
        }
    }
}