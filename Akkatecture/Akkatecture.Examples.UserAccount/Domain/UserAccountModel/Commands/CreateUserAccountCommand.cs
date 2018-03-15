using Akkatecture.Commands;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands
{
    public class CreateUserAccountCommand : Command<UserAccountAggregate,UserAccountId>
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


    //not in use yet
    public static class CreateUserAccountHandler
    {
        public static bool HandleCommand(UserAccountAggregate aggregate, UserAccountState aggregateState,
            CreateUserAccountCommand command)
        {
            var aggregateEvent = new UserAccountCreatedEvent(command.Name);

            return true;
        }
    }
}