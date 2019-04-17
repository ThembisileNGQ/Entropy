using Akkatecture.Aggregates;

namespace Domain.Model.GiftCard.Events
{
    public class RedeemedEvent : AggregateEvent<GiftCard, GiftCardId>
    {
        public int Credits { get; }
        public RedeemedEvent(int credits)
        {
            Credits = credits;
        }
    }
}