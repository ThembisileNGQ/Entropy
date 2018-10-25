using Akkatecture.Commands;

namespace Domain.Model.Car.Commands
{
    public class ChangeCarNameCommand : Command<CarAggregate, CarId>
    {
        public string Name { get; }
        public ChangeCarNameCommand(
            CarId aggreagateId,
            string name)
            : base(aggreagateId)
        {
            Name = name;
        }
    }
}