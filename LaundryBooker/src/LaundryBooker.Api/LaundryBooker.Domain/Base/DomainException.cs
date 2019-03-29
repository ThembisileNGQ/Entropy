using System;

namespace LaundryBooker.Domain.Base
{
    public class SlotIsClosedException : DomainException
    {
        public SlotIsClosedException()
        {
            Context = "bookings";
            Type = "slot-is-closed";
        }
    }
    public class UserHasOverbookedException : DomainException
    {
        public UserHasOverbookedException()
        {
            Context = "bookings";
            Type = "user-has-overbooked";
        }
    }
    public abstract class DomainException : Exception
    {
        public virtual string Context { get; set; }
        public virtual string Type { get; set; }
    }
}