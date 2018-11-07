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
    public class RoomsController : Controller
    {

        public HttpClient client = new HttpClient();
        public APIServices services = new APIServices();

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms(int? page, int? size)
        {
            string rooms;
            try
            {
                rooms = await client.GetStringAsync(services.roomsAPI + $"?page={page}&size={size}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (rooms == null)
            {
                return NotFound();
            }

            var Rooms = JsonConvert.DeserializeObject<List<Room>>(rooms);

            return Ok(Rooms);
        }

        [HttpGet("rooms/{id}")]
        public async Task<IActionResult> GetRoom(Guid id)
        {

            string room;
            try
            {
                room = await client.GetStringAsync(services.roomsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (room == null)
            {
                return NotFound();
            }

            var Room = JsonConvert.DeserializeObject<Room>(room);

            return Ok(Room);
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> PutRoom(Guid id, [FromBody] Room roomModel)
        {
            HttpResponseMessage room;
            try
            {
                room = await client.PutAsJsonAsync(services.roomsAPI + $"/{id}", roomModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (room == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{id}"));
            return Ok(room);
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> PostRoom([FromBody] Room roomModel)
        {
            HttpResponseMessage room;
            try
            {
                room = await client.PostAsJsonAsync(services.roomsAPI, roomModel);
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (room == null)
            {
                return NotFound();
            }

            //return Ok(client.GetStringAsync(customersAPI + $"/{customerModel.CustomerId}"));
            return Ok(room);

        }

        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {
            HttpResponseMessage room;
            try
            {
                room = await client.DeleteAsync(services.roomsAPI + $"/{id}");
            }
            catch
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (room == null)
            {
                return NotFound();
            }

            string bookings = null;

            try
            {
                bookings = await client.GetStringAsync(services.bookingsAPI);
            }
            catch
            {

            }

            var bk = JsonConvert.DeserializeObject<List<Booking>>(bookings);

            try
            {
                foreach (Booking entr in bk)
                {
                    if (entr.RoomId == id)
                    {

                        HttpResponseMessage booking = await client.DeleteAsync(services.bookingsAPI + $"/{entr.BookingId}");

                    }
                }
            }
            catch
            {

            }

            return Ok(room);
        }

    }
}