using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomsAPIServices.Models;
using PagedList;
using Microsoft.Extensions.Logging;

namespace RoomsAPIServices.Controllers
{
    [Produces("application/json")]
    [Route("api/Rooms")]
    public class RoomsController : Controller
    {
        private readonly RoomsContext _context;

        public RoomsController(RoomsContext context)
        {
            _context = context;
        }

        // GET: api/Rooms
        [HttpGet]
        public IEnumerable<Rooms> GetRooms(int? page, int? size)
        {

            if (page == null || size == null)
            {
                return _context.Rooms;
            }

            int pageNumber = (page ?? 1);
            int pageSize = (size ?? 1);

            return _context.Rooms.ToPagedList(pageNumber, pageSize);
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRooms([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rooms = await _context.Rooms.SingleOrDefaultAsync(m => m.RoomId == id);

            if (rooms == null)
            {
                return NotFound();
            }

            return Ok(rooms);
        }

        // PUT: api/Rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRooms([FromRoute] Guid id, [FromBody] Rooms rooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rooms.RoomId)
            {
                return BadRequest();
            }

            _context.Entry(rooms).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomsExists(id))
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

        // POST: api/Rooms
        [HttpPost]
        public async Task<IActionResult> PostRooms([FromBody] Rooms rooms)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Rooms.Add(rooms);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RoomsExists(rooms.RoomId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRooms", new { id = rooms.RoomId }, rooms);
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRooms([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rooms = await _context.Rooms.SingleOrDefaultAsync(m => m.RoomId == id);
            if (rooms == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(rooms);
            await _context.SaveChangesAsync();

            return Ok(rooms);
        }

        private bool RoomsExists(Guid id)
        {
            return _context.Rooms.Any(e => e.RoomId == id);
        }
    }
}