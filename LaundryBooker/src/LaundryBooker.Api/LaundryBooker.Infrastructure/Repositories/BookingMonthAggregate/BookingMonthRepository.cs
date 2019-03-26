using System;
using System.Threading.Tasks;
using Dapper;
using LaundryBooker.Domain.Model.BookingMonth;
using LaundryBooker.Domain.Repositories;
using Newtonsoft.Json;
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
        
        public async Task<BookingMonth> Find(BookingMonthId aggregateId)
        {
            var query = $@"
                SELECT
                    id as Id,
                    user_name as Name,
                    normalized_name as NormalizedName
                FROM laundry.users
                WHERE id = @id";
            
            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                var dataModel = await connection.QuerySingleOrDefaultAsync<BookingMonthDataModel>(query, new {id = aggregateId.Value});
                
                if (dataModel == null)
                    return null;

                var aggregate = BookingMonthMapper.From(dataModel);
                
                return aggregate;
            }
            
        }

        public async Task Upsert(BookingMonth aggregate)
        {
            var upsertCommand = $@"
                INSERT INTO laundry.bookings(
                    id,
                    bookings)
                VALUES(
                    @id,
                    @booking::jsonb)
                ON CONFLICT(id) DO UPDATE SET booking = excluded.booking;";
            
            var booking = JsonConvert.SerializeObject(aggregate);
            
            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                await connection.ExecuteAsync(upsertCommand, new BookingMonthPostgresDocument { Id = aggregate.Id.Value, Booking = booking });
            }
        }
    }
}