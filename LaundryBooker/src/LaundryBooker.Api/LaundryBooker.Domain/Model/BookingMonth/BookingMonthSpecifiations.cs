using System;
using System.Linq;
using System.Runtime.CompilerServices;
using LaundryBooker.Domain.Model.BookingMonth.Entities;
using LaundryBooker.Domain.Model.User;

namespace LaundryBooker.Domain.Model.BookingMonth
{
    public class BookingMonthSpecifiations
    {
        public static bool SlotIsOpen(BookingDay bookingDay, Slot slot)
        {
            return !bookingDay.Bookings.ContainsKey(slot);
        }

        public static bool UserCanBook(BookingMonth bookingMonth, UserId userId)
        {
            var userBookingCount = bookingMonth.BookingDays
                .SelectMany(x => x.Value.Bookings.Values.ToList())
                .Count(x => x == userId);

            return (userBookingCount < 3);
        }

        public static bool IsBookableDay(int year, int month, int day)
        {
            return DateTime.DaysInMonth(year, month) >= day && day > 0;
        }
    }
}