using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Domain.Model.Car
{
    public class CarAggregateManager: AggregateManager<CarAggregate,CarId, Command<CarAggregate, CarId>>
    {
    }
}