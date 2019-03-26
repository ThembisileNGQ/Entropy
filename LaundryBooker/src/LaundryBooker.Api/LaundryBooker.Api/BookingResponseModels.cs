using System;
using System.Collections.Generic;

namespace LaundryBooker.Api
{
    public static class BookingRespo
    public class BookingResponseModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Dictionary<int, BookingDayModel> BookingDays { get; set; }
    }

    public class BookingDayModel
    {
        public string Id { get; set; }
        public Dictionary<int, Guid> Bookings { get; set; }
    }
}
