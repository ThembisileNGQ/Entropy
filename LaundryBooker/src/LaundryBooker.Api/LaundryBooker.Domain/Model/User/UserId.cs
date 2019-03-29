using LaundryBooker.Domain.Base.Identity;

namespace LaundryBooker.Domain.Model.User
{
    public class UserId : Identity<UserId>
    {
        public UserId(string value) 
            : base(value)
        {
        }
    }
}