using SoberPath_API.Context;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Globalization;

namespace SoberPath_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpGet("GetMonthlyEvents/{socialId}/{month}/{year}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByMonth(int socialId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            var endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            return await _context.Events
                .Where(e => e.Social_Id == socialId &&
                       string.Compare(e.Date, startDate) >= 0 &&
                       string.Compare(e.Date, endDate) <= 0)
                .ToListAsync();
        }

        [HttpGet("GetDayEvents/{socialId}/{date}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByDate(int socialId, string date)
        {
            try
            {
                // Validate date format
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd");
                }

                var events = await _context.Events
                    .Where(e => e.Social_Id == socialId && e.Date == date)
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("CreateEvent")]
        public async Task<ActionResult<Event>> CreateEvent(Event calendarEvent)
        {
            // Validate date format
            if (!DateTime.TryParse(calendarEvent.Date, out _))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd");
            }

            // Validate time formats
            if (!TimeSpan.TryParse(calendarEvent.StartTime, out _) ||
                !TimeSpan.TryParse(calendarEvent.EndTime, out _))
            {
                return BadRequest("Invalid time format. Use HH:mm");
            }

            calendarEvent.IsRead = false;
            _context.Events.Add(calendarEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = calendarEvent.Id }, calendarEvent);
        }

        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var calendarEvent = await _context.Events.FindAsync(id);
            if (calendarEvent == null)
            {
                return NotFound();
            }

            _context.Events.Remove(calendarEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
