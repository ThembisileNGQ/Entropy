using System;
using LaundryBooker.Domain.Base.Identity;

namespace LaundryBooker.Domain.Model.BookingMonth
{
    public class BookingMonthId : Identity<BookingMonthId>
    {
        
        public BookingMonthId(string value) 
            : base(value)
        {
        }

        public static BookingMonthId From(DateTime dateTime)
        {
            var id = $"{dateTime.Year}-{dateTime.Month}";
            return new BookingMonthId(id);
        }
    }
}