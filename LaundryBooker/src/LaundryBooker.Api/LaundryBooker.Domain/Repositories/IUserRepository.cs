using System.Threading.Tasks;
using LaundryBooker.Domain.Model.User;

namespace LaundryBooker.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> Find(UserId aggregateId);
        Task<User> Find(string username);
    }
}