using Akkatecture.Commands;

namespace Domain.Model.Car.Commands
{
    public class CreateCarCommand : Command<CarAggregate, CarId>
    {
        public CreateCarCommand(
            CarId aggreagateId)
            : base(aggreagateId)
        {
        }
    }
}