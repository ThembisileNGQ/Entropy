using System;
using System.Threading.Tasks;
using Dapper;
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
        
        public async Task<User> Find(UserId aggregateId)
        {
            var query = $@"
                SELECT
                    id as Id,
                    display_name as Name,
                    normalized_name as NormalizedName
                FROM laundry.users
                WHERE id = @id";
            
            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                var dataModel = await connection.QuerySingleOrDefaultAsync<UserDataModel>(query, new {id = aggregateId.Value});

                if (dataModel == null)
                    return null;

                var aggregate = UserMapper.From(dataModel);
                
                return aggregate;
            }
            
        }

        public async Task<User> Find(string username)
        {
            var query = $@"
                SELECT
                    id as Id,
                    display_name as Name,
                    normalized_name as NormalizedName
                FROM laundry.users
                WHERE normalized_name = @normalizedName";

            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                var dataModel = await connection.QuerySingleOrDefaultAsync<UserDataModel>(query, new { normalizedName = username.ToUpperInvariant() });

                if (dataModel == null)
                    return null;

                var aggregate = UserMapper.From(dataModel);

                return aggregate;
            }
        }
    }
}