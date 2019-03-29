using System.Collections.Generic;
using System.Threading.Tasks;
using LaundryBooker.Domain.Model.BookingMonth;
using LaundryBooker.Domain.Model.BookingMonth.Entities;
using LaundryBooker.Domain.Model.User;
using LaundryBooker.Domain.Repositories;

namespace LaundryBooker.Api.Models
{
    public class BookingResponseModelFactory
    {
        private IUserRepository _userRepository { get; }
        public BookingResponseModelFactory(
            IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<BookingMonthResponseModel> From(BookingMonth aggregate)
        {
            return new BookingMonthResponseModel
            {
                Year = aggregate.Year,
                Month = aggregate.Month,
                BookingDays = await From(aggregate.BookingDays)
            };
        }

        private async Task<Dictionary<int, BookingDayResponseModel>> From(Dictionary<int, BookingDay> entity)
        {
            var dict = new Dictionary<int, BookingDayResponseModel>();
            foreach (var key in entity.Keys)
            {
                dict[key] = new BookingDayResponseModel
                {
                    Id = entity[key].Id.Value,
                    Day = key,
                    Bookings = await From(entity[key].Bookings)
                };
            }

            return dict;
        }

        public async Task<List<UserBookingResponseModel>> From(Dictionary<Slot, UserId> entities)
        {
            var userBookings = new List<UserBookingResponseModel>();

            foreach (var key in entities.Keys)
            {
                var slot = (int) key;
                var userBooking = new UserBookingResponseModel
                {
                    Id = entities[key].Value,
                    Slot = slot,
                    Name = (await _userRepository.Find(entities[key])).Name
                };
                
                userBookings.Add(userBooking);
            }
    
            return userBookings;
        } 

        public async Task<UserBookingResponseModel> From(UserId userId,int slot)
        {
            var user = await _userRepository.Find(userId);

            return new UserBookingResponseModel
            {
                Slot = slot,
                Id = user.Id.Value,
                Name = user.Name
            };
        }
    }
}