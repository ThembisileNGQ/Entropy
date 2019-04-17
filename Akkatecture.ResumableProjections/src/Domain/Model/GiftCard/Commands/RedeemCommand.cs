using Akkatecture.Commands;

namespace Domain.Model.GiftCard.Commands
{
    public class RedeemCommand : Command<GiftCard, GiftCardId>
    {
        public int Credits { get; }
        public RedeemCommand(
            GiftCardId aggreagateId,
            int credits)
            : base(aggreagateId)
        {
            Credits = credits;
        }
    }
}