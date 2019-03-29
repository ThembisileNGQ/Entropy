using System;
using System.Collections.Generic;
using System.Security.Claims;
using LaundryBooker.Domain.Base.AggregateRoot;

namespace LaundryBooker.Domain.Model.User
{
    public class User : AggregateRoot<UserId>
    {
        public string Name { get; }
        
        public User(
            UserId id,
            string name) 
            : base(id)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentException(nameof(name));

            Name = name;
        }

    }
}