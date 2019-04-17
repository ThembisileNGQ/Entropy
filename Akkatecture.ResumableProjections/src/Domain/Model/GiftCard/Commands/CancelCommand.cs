using Akkatecture.Commands;

namespace Domain.Model.GiftCard.Commands
{
    public class CancelCommand: Command<GiftCard, GiftCardId>
    {
        public CancelCommand(
            GiftCardId aggreagateId)
            : base(aggreagateId)
        {
        }
    }
}
