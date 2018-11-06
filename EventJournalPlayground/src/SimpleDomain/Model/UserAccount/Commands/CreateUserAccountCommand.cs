using Akkatecture.Commands;

namespace SimpleDomain.Model.UserAccount.Commands
{
    public class CreateUserAccountCommand : Command<UserAccountAggregate, UserAccountId>
    {
        public string Name { get; }
        public CreateUserAccountCommand(
            UserAccountId aggregateId,
            string name)
            : base(aggregateId)
        {
            Name = name;
        }
    }
    
}