using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaundryBooker.Domain.Model.BookingMonth;
using LaundryBooker.Domain.Model.BookingMonth.Entities;
using LaundryBooker.Domain.Model.User;
using LaundryBooker.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LaundryBooker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private IBookingMonthRepository _repository;

        public BookingsController(
            IBookingMonthRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{year:int}/{month:int}", Name = "GetBookings")]
        [Authorize("bookings.read")]
        public async Task<ActionResult<BookingMonth>> GetBookings(int year, int month)
        {
            var bookingMonthId = BookingMonthId.From(new DateTime(year, month,1));
            var bookingMonth = await _repository.Find(bookingMonthId);

            if (bookingMonth == null)
                return NotFound();

            return bookingMonth;
        }

        [HttpGet("{year:int}/{month:int}/{day:int}/{slot:int}", Name = "GetBookingSlot")]
        [Authorize("bookings.read")]
        public async Task<ActionResult<object>> GetBookingSlot(int year, int month, int day, int slot)
        {
            var bookingMonthId = BookingMonthId.From(new DateTime(year, month, day));
            var bookingMonth = await _repository.Find(bookingMonthId);

            if (bookingMonth == null)
                return NotFound();

            var slotEnum = (Slot)slot;

            if (bookingMonth.BookingDays.ContainsKey(day) && bookingMonth.BookingDays[day].Bookings.ContainsKey(slotEnum))
                return new { userId = bookingMonth.BookingDays[day].Bookings[slotEnum].Value };

            return NotFound();
        }

        [HttpPost("{year:int}/{month:int}/{day:int}", Name = "CreateBooking")]
        [Authorize("bookings.write")]
        public async Task<IActionResult> PostBooking(
            [FromRoute]int year,
            [FromRoute]int month,
            [FromRoute]int day,
            [FromBody]BookingsInputModel input)
        {
            var bookingMonthId = BookingMonthId.From(new DateTime(year, month, day));
            var bookingMonth = await _repository.Find(bookingMonthId);

            if (bookingMonth == null)
                bookingMonth = new BookingMonth(bookingMonthId, year, month, new Dictionary<int, BookingDay>());

            var userId = UserId.With(HttpContext.GetUserId().Value);
            var slot = (Slot)input.Slot;

            bookingMonth.AddBooking(userId, day, slot);

            await _repository.Upsert(bookingMonth);

            return CreatedAtAction("GetBookingSlot", new { year = year, month = month, day = day, slot = input.Slot, }, new { userId = bookingMonth.BookingDays[day].Bookings[slot].Value });
        }


        [HttpDelete("{year:int}/{month:int}/{day:int}/{slot:int}", Name = "DeleteBookingSlot")]
        public void DeleteBooking(int id)
        {
        }
    }

    public class BookingsInputModel
    {
        public int Slot { get; set; }
    }
}