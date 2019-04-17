using Akkatecture.Aggregates;
using Domain.Model.GiftCard.Events;

namespace Domain.Model.GiftCard
{
    public class GiftCardState: AggregateState<GiftCard,GiftCardId>,
        IApply<IssuedEvent>,
        IApply<RedeemedEvent>,
        IApply<CancelledEvent>
    {
        public int Credits { get; private set; }
        

        public void Apply(IssuedEvent aggregateEvent)
        {
            Credits = aggregateEvent.Credits;
        }
        
        public void Apply(RedeemedEvent aggregateEvent)
        {
            Credits = Credits - aggregateEvent.Credits;
        }
        
        public void Apply(CancelledEvent aggregateEvent)
        {
            Credits = 0;
        }
    }
}