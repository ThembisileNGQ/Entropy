using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Domain.Model.Car
{
    public class GiftCardManager: AggregateManager<GiftCard, GiftCardId, Command<GiftCard, GiftCardId>>
    {
    }
}