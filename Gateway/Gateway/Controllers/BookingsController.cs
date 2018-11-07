﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gateway.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Gateway.Controllers
{
    [Route("")]
    public class BookingsController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        [HttpGet("bookings")]
        public async Task<IActionResult> GetBookings(int? page, int? size)
        {
            string bookings;
            try
            {
                bookings = await client.GetStringAsync(services.bookingsAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (bookings == null)
            {
                return NotFound();
            }

            var Bookings = JsonConvert.DeserializeObject<List<Booking>>(bookings);

            return Ok(Bookings);
        }

        [HttpGet("bookings/{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {

            string booking;
            try
            {
                booking = await client.GetStringAsync(services.bookingsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            var Booking = JsonConvert.DeserializeObject<Booking>(booking);

            return Ok(Booking);
        }

        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> PutBooking(Guid id, [FromBody] Booking bookingModel)
        {
            HttpResponseMessage booking;
            try
            {
                booking = await client.PutAsJsonAsync(services.bookingsAPI + $"/{id}", bookingModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{id}"));
            return Ok(booking);
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> PostBooking([FromBody] Booking bookingModel)
        {
            string room = null;
            string customer = null;

            try
            {
                room = await client.GetStringAsync(services.roomsAPI + $"/{bookingModel.RoomId}");
                customer = await client.GetStringAsync(services.customersAPI + $"/{bookingModel.CustomerId}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (room == null || customer == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            HttpResponseMessage booking;

            try
            {
                booking = await client.PostAsJsonAsync(services.bookingsAPI, bookingModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{customerModel.CustomerId}"));
            return Ok(booking);

        }

        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            HttpResponseMessage booking;
            try
            {
                booking = await client.DeleteAsync(services.bookingsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

    }
}