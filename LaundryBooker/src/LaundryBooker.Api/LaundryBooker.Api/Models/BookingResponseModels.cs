using System.Collections.Generic;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;

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
        public List<UserBookingResponseModel> Bookings { get; set; }
    }

    public class UserBookingResponseModel
    {
        public int Slot { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UserHasOverBookedProblemDetails : StatusCodeProblemDetails
    {
        public UserHasOverBookedProblemDetails()
            : base(StatusCodes.Status400BadRequest)
        {
            Type = "https://mrwhite-laundry.com/probs/user-has-overbooked";
            Title = "user has overbooked";
            Detail = "You have overbooked for that month";
        }
    }
    
    
    public class SlotIsClosedProblemDetails : StatusCodeProblemDetails
    {
        public SlotIsClosedProblemDetails()
            : base(StatusCodes.Status400BadRequest)
        {
            Type = "https://mrwhite-laundry.com/probs/slot-is-closed";
            Title = "user has double booked";
            Detail = "that slot is already booked";
        }
    }
}
