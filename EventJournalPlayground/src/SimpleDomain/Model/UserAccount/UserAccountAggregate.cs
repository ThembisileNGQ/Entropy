using System;
using Akkatecture.Aggregates;
using SimpleDomain.Model.UserAccount.Commands;
using SimpleDomain.Model.UserAccount.Events;

namespace SimpleDomain.Model.UserAccount
{
    public class A
    {
        public string Hey { get; set; }
    }
    public class UserAccountAggregate : AggregateRoot<UserAccountAggregate,UserAccountId,UserAccountState>
    {
        public UserAccountAggregate(UserAccountId id)
            : base(id)
        {
            Command<CreateUserAccountCommand>(Execute);
            Command<UserAccountChangeNameCommand>(Execute);
        }
        public bool Execute(CreateUserAccountCommand command)
        {
            Create(command.Name);
            return true;
        }

        public bool Execute(UserAccountChangeNameCommand command)
        {
            ChangeName(command.Name);
            return true;
        }
        
        private void Create(string name)
        {
            if (IsNew)
            {
                Emit(new UserAccountCreatedEvent(name));
            }
            else
            {
                //signal domain error, aggregate already exists.
            }
        }

        private void ChangeName(string name)
        {
            if (!IsNew)
            {
                Emit(new UserAccountNameChangedEvent(name));   
            }
            else
            {
                //signal domain error, aggregate doesnt exist.
            }
        }
        
    }
}