using System.Collections.Generic;
using LaundryBooker.Domain.Base.Entity;
using LaundryBooker.Domain.Model.User;

namespace LaundryBooker.Domain.Model.BookingMonth.Entities
{
    public class BookingDay : Entity<BookingDayId>
    {    
        public Dictionary<Slot,UserId> Bookings { get; }
        
        public BookingDay(BookingDayId id, Dictionary<Slot,UserId> bookings) 
            : base(id)
        {
            Bookings = bookings;
        }

        public void AddBooking(Slot slot, UserId userId)
        {
            Bookings.Add(slot, userId);
        }
    }

    public enum Slot
    {
        One = 1,
        Two = 2,
        Three = 3,
    }
}