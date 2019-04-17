using Akkatecture.Aggregates;
using Akkatecture.Commands;

namespace Domain.Model.GiftCard
{
    public class GiftCardManager: AggregateManager<GiftCard, GiftCardId, Command<GiftCard, GiftCardId>>
    {
    }
}