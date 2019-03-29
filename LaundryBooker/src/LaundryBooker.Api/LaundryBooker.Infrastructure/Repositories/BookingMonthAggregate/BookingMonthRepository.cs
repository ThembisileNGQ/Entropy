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
                    booking as Booking
                FROM laundry.bookings
                WHERE id = @id";
            
            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                var pgDocument = await connection.QuerySingleOrDefaultAsync<BookingMonthPostgresDocument>(query, new {id = aggregateId.Value});

                if (pgDocument == null)
                    return null;

                var dataModel = JsonConvert.DeserializeObject<BookingMonthDataModel>(pgDocument.Booking);

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
                    booking)
                VALUES(
                    @id,
                    @booking::jsonb)
                ON CONFLICT(id) DO UPDATE SET booking = excluded.booking;";

            var dataModel = BookingMonthMapper.From(aggregate);

            var booking = JsonConvert.SerializeObject(dataModel);
            
            using (var connection = new NpgsqlConnection(_options.ConnectionString))
            {
                await connection.ExecuteAsync(upsertCommand, new BookingMonthPostgresDocument { Id = aggregate.Id.Value, Booking = booking });
            }
        }
    }
}