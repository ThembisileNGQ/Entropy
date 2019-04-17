using Akkatecture.Aggregates;

namespace Domain.Model.GiftCard.Events
{
    public class CancelledEvent : AggregateEvent<GiftCard, GiftCardId>
    {
    }
}