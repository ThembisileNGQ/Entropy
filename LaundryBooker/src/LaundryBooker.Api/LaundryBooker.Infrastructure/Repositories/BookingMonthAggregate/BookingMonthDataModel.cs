using System.Collections.Generic;
using Newtonsoft.Json;

namespace LaundryBooker.Infrastructure.Repositories.BookingMonthAggregate
{
    public class BookingMonthPostgresDocument
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("booking")]
        public string Booking { get; set; }
    }
    public class BookingMonthDataModel
    {
        public string Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public Dictionary<int, BookingDayDataModel> BookingDays { get; set; }
    }

    public class BookingDayDataModel
    {
        public string Id { get; set; }
        public Dictionary<SlotEnumModel, string> Bookings { get; set; }
    }
    
    public enum SlotEnumModel
    {
        One = 1,
        Two = 2,
        Three = 3,
    }
}