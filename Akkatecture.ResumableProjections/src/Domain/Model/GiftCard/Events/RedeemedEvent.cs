using Akkatecture.Aggregates;

namespace Domain.Model.Car.Events
{
    public class RedeemedEvent : AggregateEvent<GiftCard, GiftCardId>
    {
        public string Name { get; }
        public RedeemedEvent(string name)
        {
            Name = name;
        }
    }
}