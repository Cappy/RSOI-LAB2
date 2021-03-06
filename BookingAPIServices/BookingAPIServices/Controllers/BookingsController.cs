﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingAPIServices.Models;
using PagedList;
using Microsoft.Extensions.Logging;

namespace BookingAPIServices.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BookingsController : Controller
    {
        private readonly BookingContext _context;

        public BookingsController(BookingContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet]
        public IEnumerable<Booking> GetBookings(int? page, int? size)
        {
            if (page == null || size == null)
            {
                return _context.Booking;
            }

            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 1);

            return _context.Booking.ToPagedList(pageNumber, pageSize);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        // GET bookings by room ID
        [HttpGet("getbyroomid/{id}")]
        public IEnumerable<Booking> GetByRoomID([FromRoute] Guid id)
        {
            return _context.Booking.Where(m => m.RoomId == id);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking([FromRoute] Guid id, [FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != booking.BookingId)
            {
                return BadRequest();
            }

            if (booking.CustomerId == null || booking.RoomId == null || booking.BookingId == null)
            {
                return BadRequest();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> PostBooking([FromBody] Booking booking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Booking.Add(booking);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookingExists(booking.BookingId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBooking", new { id = booking.BookingId }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var booking = await _context.Booking.SingleOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(booking);
        }




        private bool BookingExists(Guid id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}