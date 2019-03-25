using System;
using System.Threading.Tasks;
using LaundryBooker.Domain.Model.User;
using LaundryBooker.Domain.Repositories;
using Npgsql;

namespace LaundryBooker.Infrastructure.Repositories.UsersAggregate
{
    public class UserRepository : IUserRepository
    {
        private PostgresOptions _options { get; }
        
        public UserRepository(PostgresOptions options)
        {
            _options = options;
        }
        
        public Task<User> Find(UserId aggregateId)
        {
            var query = $@"
                SELECT
                    id as Id,
                    user_name as Name,
                    normalized_name as NormalizedName
                WHERE id = @id";
            
            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                
            }
            
            throw new NotImplementedException();
        }
    }
}