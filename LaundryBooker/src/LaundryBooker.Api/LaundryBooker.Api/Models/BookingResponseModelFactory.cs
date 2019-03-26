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
                    Bookings = await From(entity[key].Bookings)
                };
            }

            return dict;
        }

        public async Task<Dictionary<int, UserResponseModel>> From(Dictionary<Slot, UserId> entities)
        {
            var dict = new Dictionary<int,UserResponseModel>();

            foreach (var key in entities.Keys)
            {
                var user = await _userRepository.Find(entities[key]);
                dict[(int)key] = new UserResponseModel
                {
                    Id = entities[key].Value,
                    Name = (await _userRepository.Find(entities[key])).Name
                };
            }

            return dict;
        } 

        public async Task<UserResponseModel> From(UserId userId)
        {
            var user = await _userRepository.Find(userId);

            return new UserResponseModel
            {
                Id = user.Id.Value,
                Name = user.Name
            };
        }
    }
}