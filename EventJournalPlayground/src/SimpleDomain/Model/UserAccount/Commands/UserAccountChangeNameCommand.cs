using Akkatecture.Commands;

namespace SimpleDomain.Model.UserAccount.Commands
{
    public class UserAccountChangeNameCommand : Command<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public UserAccountChangeNameCommand(
            UserAccountId aggreagateId,
            string name)
            : base(aggreagateId)
        {
            Name = name;
        }
    }
}