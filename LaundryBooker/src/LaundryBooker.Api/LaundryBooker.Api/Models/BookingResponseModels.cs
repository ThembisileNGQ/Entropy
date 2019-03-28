using System.Collections.Generic;
using System.Threading.Tasks;
using LaundryBooker.Domain.Model.BookingMonth;
using LaundryBooker.Domain.Model.BookingMonth.Entities;
using LaundryBooker.Domain.Model.User;
using LaundryBooker.Domain.Repositories;

namespace LaundryBooker.Api.Models
{
    
    public class BookingMonthResponseModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Dictionary<int, BookingDayResponseModel> BookingDays { get; set; }
    }

    public class BookingDayResponseModel
    {
        public string Id { get; set; }
        public int Day {get;set;}
        public Dictionary<int, UserResponseModel> Bookings { get; set; }
    }

    public class UserResponseModel
    {
        public int Slot { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
