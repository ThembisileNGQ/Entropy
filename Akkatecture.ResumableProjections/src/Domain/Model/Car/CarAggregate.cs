using System.Collections.Generic;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Domain.Model.Car.Commands;
using Domain.Model.Car.Events;

namespace Domain.Model.Car
{
    public class CarAggregate : AggregateRoot<CarAggregate,CarId,CarState>
    {
        public CarAggregate(CarId id)
            : base(id)
        {
            Command<CreateCarCommand>(Handle);
            Command<ChangeCarNameCommand>(Handle);
        }

        private bool Handle(CreateCarCommand command)
        {
            if (IsNew)
            {
                Emit(new CarCreatedEvent());
                Sender.Tell(new SuccessExecutionResult(),Self);
            }
            else
            {
                Sender.Tell(new FailedExecutionResult(new List<string> {"aggregate is already created"}),Self);
            }

            return true;
        }
        
        private bool Handle(ChangeCarNameCommand command)
        {
            if (!IsNew)
            {
                Emit(new CarNameChangedEvent(command.Name));
                Sender.Tell(new SuccessExecutionResult(),Self);
            }
            else
            {
                Sender.Tell(new FailedExecutionResult(new List<string> {"aggregate is not created"}),Self);
            }
            
            return true;
        }
    }
}