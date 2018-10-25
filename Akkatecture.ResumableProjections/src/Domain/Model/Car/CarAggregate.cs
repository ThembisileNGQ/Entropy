using Akkatecture.Aggregates;

namespace Domain.Model.Car
{
    public class CarAggregate : AggregateRoot<CarAggregate,CarId,CarState>
    {
        public CarAggregate(CarId id)
            : base(id)
        {
            
        }
    }
}