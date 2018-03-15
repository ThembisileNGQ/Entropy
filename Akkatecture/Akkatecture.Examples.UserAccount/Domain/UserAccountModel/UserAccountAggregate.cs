using System;
using Akkatecture.Aggregates;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Commands;
using Akkatecture.Examples.UserAccount.Domain.UserAccountModel.Events;

namespace Akkatecture.Examples.UserAccount.Domain.UserAccountModel
{
    public class UserAccountAggregate : AggregateRoot<UserAccountAggregate,UserAccountId,UserAccountState>
    {
        public UserAccountAggregate(UserAccountId id)
            : base(id)
        {
            State = new UserAccountState();

            Become(UserAccount);
        }

        public void UserAccount()
        {
            Command<CreateUserAccountCommand>(Handle);
        }

        public bool Handle(CreateUserAccountCommand command)
        {
            Create(command.Name);
            return true;
        }

        public void Create(string name)
        {
            if (Version <= 0)
            {
                Emit(new UserAccountCreatedEvent(name));
            }        
        }
    }
}