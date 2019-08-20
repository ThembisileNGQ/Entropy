using System.Collections.Generic;
using Akkatecture.Aggregates;
using Akkatecture.Aggregates.ExecutionResults;
using Akkatecture.Extensions;
using Domain.Model.GiftCard.Commands;
using Domain.Model.GiftCard.Events;

namespace Domain.Model.GiftCard
{
    public class GiftCard : AggregateRoot<GiftCard,GiftCardId,GiftCardState>,
        IExecute<IssueCommand>,
        IExecute<RedeemCommand>,
        IExecute<CancelCommand>
    {
        public GiftCard(GiftCardId id)
            : base(id)
        {
        }
        
        
        
        public bool Execute(IssueCommand command)
        {
            if (IsNew)
            {
                Emit(new IssuedEvent(command.Credits));
                Sender.Tell(new SuccessExecutionResult(),Self);
            }
            else
            {
                Log.Error($"{command.GetType().PrettyPrint()} has failed ");
                Sender.Tell(new FailedExecutionResult(new List<string> {"aggregate is not created"}),Self);
            }
            
            return true;
        }

        public bool Execute(RedeemCommand command)
        {
            if (!IsNew && State.Credits >= command.Credits)
            {
                Emit(new RedeemedEvent(command.Credits));
                Sender.Tell(new SuccessExecutionResult(),Self);
            }
            else
            {
                Log.Error($"{command.GetType().PrettyPrint()} has failed ");
                Sender.Tell(new FailedExecutionResult(new List<string> {"aggregate is already created"}),Self);
            }

            return true;
        }
        public bool Execute(CancelCommand command)
        {
            if (!IsNew)
            {
                Emit(new CancelledEvent());
                Sender.Tell(new SuccessExecutionResult(), Self);
            }
            else
            {
                Log.Error($"{command.GetType().PrettyPrint()} has failed ");
                Sender.Tell(new FailedExecutionResult(new List<string> {"aggregate is not created"}),Self);
            }
            
            return true;
        }
    }
}