using System;
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
    public class AggregationController : Controller
    {
        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        [HttpGet("booking-with-info/{id}")]
        public async Task<IActionResult> GetBookingWithInfo(Guid id)
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

            // var bk = JsonConvert.DeserializeObject<List<Booking>>(booking); //список json'ов
             Booking bk = JsonConvert.DeserializeObject<Booking>(booking);

            string customer;
            string room;

            try
            {
                customer = await client.GetStringAsync(services.customersAPI + $"/{bk.CustomerId}");
                room = await client.GetStringAsync(services.roomsAPI + $"/{bk.RoomId}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (customer == null || room == null)
            {
                return NotFound();
            }

            Customer cs = JsonConvert.DeserializeObject<Customer>(customer);
            Room rm = JsonConvert.DeserializeObject<Room>(room);

            var result = new BookingWithInfo
            {
                BookingId = bk.BookingId,
                customer = new Customer
                {
                    CustomerId = cs.CustomerId,
                    Name = cs.Name,
                    Surname = cs.Surname,
                    PhoneNumber = cs.PhoneNumber
                },
                room = new Room
                {
                    RoomId = rm.RoomId,
                    Number = rm.Number,
                    Cost = rm.Cost
                }
            };

            var json = JsonConvert.SerializeObject(result);

            return Ok(json);
        }
    }
}
