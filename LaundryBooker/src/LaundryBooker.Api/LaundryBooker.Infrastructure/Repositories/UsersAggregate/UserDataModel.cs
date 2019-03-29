using System;

namespace LaundryBooker.Infrastructure.Repositories.UsersAggregate
{
    public class UserDataModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}