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
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace Gateway.Controllers
{
    [Route("api/")]
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
            string err = string.Empty;

            if (roomModel.Number <= 0)
            {
                err += "Number must be greater than 0. ";
            }
            if (roomModel.Cost <= 0)
            {
                err += "Cost must be greater than 0.";
            }

            if (err != "")
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err
                });
            }

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

            return Ok();
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> PostRoom([FromBody] Room roomModel)
        {
            HttpResponseMessage room = null;
            string err = string.Empty;

            Guid id = Guid.NewGuid();
            roomModel.RoomId = id;


            if (roomModel.Number <= 0)
                {
                    err += "Number must be greater than 0. ";
                }
            if (roomModel.Cost <= 0)
                {
                    err += "Cost must be greater than 0.";
                }

            if (err != "")
            {
                return StatusCode(StatusCodes.Status400BadRequest, new
                {
                    err
                });
            }

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
            return Ok(roomModel);

        }

        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(Guid id)
        {

            Room RoomBU = new Room();
            HttpResponseMessage roomBackup = null;
            
            //getting room info before deleteing
            try
            {
                roomBackup = await client.GetAsync(services.gatewayAPI + $"/rooms/{id}");
                RoomBU = await roomBackup.Content.ReadAsAsync<Room>();
            }
            catch
            {
                if (roomBackup.StatusCode != HttpStatusCode.ServiceUnavailable)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        err = string.Format("Room with ID {0} is not found in the DB, nothing to delete", id)
                    });
                }
            }


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

            HttpResponseMessage bookings = null;

            try
            {
                bookings = await client.GetAsync(services.bookingsAPI);
            }
            catch
            {
                var restoreRoom = await client.PostAsJsonAsync(services.gatewayAPI + "/rooms", RoomBU);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Rolled back: Booking service is unavailable."
                });

            }

            var bk = await bookings.Content.ReadAsAsync<List<Booking>>();

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
                var restoreRoom = await client.PostAsJsonAsync(services.gatewayAPI + "/rooms", RoomBU);
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    err = "Rolled back: Booking service is unavailable."
                });
            }

            return Ok(room);
        }

    }
}