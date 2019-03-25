using System.Collections.Generic;
using System.Threading.Tasks;
using LaundryBooker.Domain.Model.BookingMonth;

namespace LaundryBooker.Domain.Repositories
{
    public interface IBookingMonthRepository
    {
        Task<BookingMonth> Find(BookingMonthId aggregateId);
    }
}