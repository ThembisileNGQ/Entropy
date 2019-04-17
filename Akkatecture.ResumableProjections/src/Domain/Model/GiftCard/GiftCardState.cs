using Akkatecture.Aggregates;
using Domain.Model.Car.Events;

namespace Domain.Model.Car
{
    public class GiftCardState: AggregateState<GiftCard,GiftCardId>,
        IApply<CancelledEvent>,
        IApply<RedeemedEvent>
    {
        public string Name { get; private set; }
        
        public void Apply(CancelledEvent aggregateEvent)
        {
            
        }

        public void Apply(RedeemedEvent aggregateEvent)
        {
            Name = aggregateEvent.Name;
        }
    }
}