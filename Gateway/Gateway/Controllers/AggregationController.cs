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
using System.Net;

namespace Gateway.Controllers
{
    [Route("api/")]
    public class AggregationController : Controller
    {
        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        //all bookings of this customer
        [HttpGet("customers/{id}/bookings")]
        public async Task<IActionResult> GetBookingsOfCustomer(Guid id)
        {
            //HttpResponseMessage bookings;
            //Booking bookings;
            string bookings;
            try
            {
                bookings = await client.GetStringAsync(services.bookingsAPI);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (bookings == null)
            {
                return NotFound();
            }

            var bk = JsonConvert.DeserializeObject<List<Booking>>(bookings);
            bk = bk.Where(x => x.CustomerId == id).ToList();

            return Ok(bk);

        }

        [HttpGet("booking-with-info/{id}")]
        public async Task<IActionResult> GetBookingWithInfo(Guid id)
        {
            HttpResponseMessage booking;
            try
            {
                booking = await client.GetAsync(services.bookingsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Booking service is unavailable"
                });
            }

            if (booking.StatusCode == HttpStatusCode.NotFound)
            {
                return StatusCode(StatusCodes.Status404NotFound, new
                {
                    err = string.Format("Booking with ID {0} is not found in the DB", id)
                });
            }

            // var bk = JsonConvert.DeserializeObject<List<Booking>>(booking);
            //Booking bk = JsonConvert.DeserializeObject<Booking>(booking.Content.);
            Booking bk = await booking.Content.ReadAsAsync<Booking>();

            string customer = "";
            string room = "";

            Customer csNull = new Customer();
            Room rmNull = new Room();

            try
            {
                customer = await client.GetStringAsync(services.customersAPI + $"/{bk.CustomerId}");
            }
            catch
            {
                csNull = new Customer()
                {
                    CustomerId = Guid.Empty,
                    Name = "[service unavailable]",
                    Surname = "",
                    PhoneNumber = "[service unavailable]"
                };
            }

            try
            {
                room = await client.GetStringAsync(services.roomsAPI + $"/{bk.RoomId}");
            }
            catch
            {

                rmNull = new Room()
                {
                    RoomId = Guid.Empty,
                    Number = -1,
                    Cost = -0.0
                };
            }


            Customer cs = JsonConvert.DeserializeObject<Customer>(customer);
            Room rm = JsonConvert.DeserializeObject<Room>(room);

            if (customer == string.Empty)
            {
                cs = csNull;
            }

            if (room == string.Empty)
            {
                rm = rmNull;
            }

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

            return Ok(result);
        }

        [HttpGet("bookings-with-info")]
        public async Task<IActionResult> GetBookings(int? page, int? size)
        {
            HttpResponseMessage bookings;
            try
            {
                bookings = await client.GetAsync(services.bookingsAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Booking service is unavailable (503) [API message]"
                });
            }

            if (bookings == null)
            {
                return NotFound();
            }

            // var bk = JsonConvert.DeserializeObject<List<Booking>>(booking);
            //var Bookings = JsonConvert.DeserializeObject<List<Booking>>(bookings);

            var Bookings = await bookings.Content.ReadAsAsync<List<Booking>>();

            string customer = "";
            string room = "";

            var result = new List<BookingWithInfo>();


            Customer csNull = new Customer();
            Room rmNull = new Room();

            foreach (Booking bk in Bookings)
            {
                try
                {
                    customer = await client.GetStringAsync(services.customersAPI + $"/{bk.CustomerId}");
                }
                catch
                {
                    csNull = new Customer()
                    {
                        CustomerId = Guid.Empty,
                        Name = "[service unavailable]",
                        Surname = "",
                        PhoneNumber = "[service unavailable]"
                    };
                }

                try
                {
                    room = await client.GetStringAsync(services.roomsAPI + $"/{bk.RoomId}");
                }
                catch
                {

                    rmNull = new Room()
                    {
                        RoomId = Guid.Empty,
                        Number = -1,
                        Cost = -0.0
                    };
                }

                
                Customer cs = JsonConvert.DeserializeObject<Customer>(customer);
                Room rm = JsonConvert.DeserializeObject<Room>(room);

                if (customer == string.Empty)
                {
                    cs = csNull;
                }

                if (room == string.Empty)
                {
                    rm = rmNull;
                }

                var entry = new BookingWithInfo
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

                result.Add(entry);
                    
            }

            return Ok(result);
        }
    }
}
