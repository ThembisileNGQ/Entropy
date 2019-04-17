using Akkatecture.Aggregates;

namespace Domain.Model.GiftCard.Events
{
    public class IssuedEvent : AggregateEvent<GiftCard, GiftCardId>
    {
        public int Credits { get; }
        public IssuedEvent(int credits)
        {
            Credits = credits;
        }
    }
}
