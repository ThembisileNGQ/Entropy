using System;
using LaundryBooker.Domain.Base.Identity;

namespace LaundryBooker.Domain.Model.BookingMonth.Entities
{
    public class BookingDayId : Identity<BookingDayId>
    {
        public BookingDayId(string value) 
            : base(value)
        {
        }
        
        public static BookingDayId From(DateTime dateTime)
        {
            var id = $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day}";
            
            return new BookingDayId(id);
        }
    }
}