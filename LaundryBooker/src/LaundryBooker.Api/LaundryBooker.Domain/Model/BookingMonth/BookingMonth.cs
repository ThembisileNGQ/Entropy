using System;
using System.Collections.Generic;
using LaundryBooker.Domain.Base.AggregateRoot;
using LaundryBooker.Domain.Model.BookingMonth.Entities;
using LaundryBooker.Domain.Model.User;
using static LaundryBooker.Domain.Model.BookingMonth.BookingMonthSpecifiations;

namespace LaundryBooker.Domain.Model.BookingMonth
{
    public class BookingMonth : AggregateRoot<BookingMonthId>
    {
        public int Year { get; }
        public int Month { get; }
        public Dictionary<int, BookingDay> BookingDays { get; }
        
        public BookingMonth(
            BookingMonthId id,
            int year,
            int month,
            Dictionary<int, BookingDay> bookingDays) 
            : base(id)
        {
            if(year < 2019)
                throw new ArgumentException(nameof(year));
            if(month > 12 || month < 1)
                throw new ArgumentException(nameof(month));
            
            Year = year;
            Month = month;
            BookingDays = bookingDays;
        }

        public void AddBooking(UserId userId, int day, Slot slot)
        {
            if (UserCanBook(this, userId) && IsBookableDay(Year,Month,day))
            {
                if (BookingDays.ContainsKey(day))
                {
                    var bookingDay = BookingDays[day];

                    if (SlotIsOpen(bookingDay, slot))
                    {
                        bookingDay.AddBooking(slot,userId);
                    }
                }
                else
                {
                    var bookingDayId = BookingDayId.From(new DateTime(Year, Month, day));
                    var bookingDay = new BookingDay(bookingDayId, new Dictionary<Slot, UserId>());
                    
                    bookingDay.AddBooking(slot,userId);
                    BookingDays.Add(day,bookingDay);
                    
                }
            }
            
        }
        
        
    }
}