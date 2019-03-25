using System;
using System.Threading.Tasks;
using LaundryBooker.Domain.Model.BookingMonth;
using LaundryBooker.Domain.Repositories;
using Npgsql;

namespace LaundryBooker.Infrastructure.Repositories.BookingMonthAggregate
{
    public class BookingMonthRepository : IBookingMonthRepository
    {
        private PostgresOptions _options { get; }
        
        public BookingMonthRepository(PostgresOptions options)
        {
            _options = options;
        }

        
        public Task<BookingMonth> Find(BookingMonthId aggregateId)
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