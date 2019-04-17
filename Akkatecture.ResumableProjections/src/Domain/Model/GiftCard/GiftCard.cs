using System.Collections.Generic;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Domain.Model.Car.Commands;
using Domain.Model.Car.Events;

namespace Domain.Model.Car
{
    public class GiftCard : AggregateRoot<GiftCard,GiftCardId,GiftCardState>
    {
        public GiftCard(GiftCardId id)
            : base(id)
        {
            Command<RedeemCommand>(Handle);
            Command<IssueCommand>(Handle);
        }

        private bool Handle(RedeemCommand command)
        {
            if (IsNew)
            {
                Emit(new CancelledEvent());
                Sender.Tell(new SuccessExecutionResult(),Self);
            }
            else
            {
                Sender.Tell(new FailedExecutionResult(new List<string> {"aggregate is already created"}),Self);
            }

            return true;
        }
        
        private bool Handle(IssueCommand command)
        {
            if (!IsNew)
            {
                Emit(new RedeemedEvent(command.Name));
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