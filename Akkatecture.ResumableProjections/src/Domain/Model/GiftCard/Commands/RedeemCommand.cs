using Akkatecture.Commands;

namespace Domain.Model.Car.Commands
{
    public class RedeemCommand : Command<GiftCard, GiftCardId>
    {
        public RedeemCommand(
            GiftCardId aggreagateId)
            : base(aggreagateId)
        {
        }
    }
}