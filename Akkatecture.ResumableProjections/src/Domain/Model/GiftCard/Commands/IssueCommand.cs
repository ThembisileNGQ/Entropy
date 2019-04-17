using Akkatecture.Commands;

namespace Domain.Model.GiftCard.Commands
{
    public class IssueCommand : Command<GiftCard, GiftCardId>
    {
        public int Credits { get; }
        public IssueCommand(
            GiftCardId aggreagateId,
            int credits)
            : base(aggreagateId)
        {
            Credits = credits;
        }
    }
}