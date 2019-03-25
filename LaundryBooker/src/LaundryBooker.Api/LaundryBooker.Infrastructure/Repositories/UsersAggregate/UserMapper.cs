using System;
using LaundryBooker.Domain.Model.User;

namespace LaundryBooker.Infrastructure.Repositories.UsersAggregate
{
    public static class UserMapper
    {
        public static User From(UserDataModel dataModel)
        {
            var aggregateId = UserId.With(dataModel.Id);
            
            return new User(aggregateId,dataModel.Name);
        }

    }
}