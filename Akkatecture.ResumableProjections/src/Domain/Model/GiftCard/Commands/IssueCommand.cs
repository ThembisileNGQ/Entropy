using Akkatecture.Commands;

namespace Domain.Model.Car.Commands
{
    public class IssueCommand : Command<GiftCard, GiftCardId>
    {
        public string Name { get; }
        public IssueCommand(
            GiftCardId aggreagateId,
            string name)
            : base(aggreagateId)
        {
            Name = name;
        }
    }
}