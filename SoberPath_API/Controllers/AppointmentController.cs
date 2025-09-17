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
    public class AppointmentsController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        // GET: api/appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAppointments()
        {
            return await _context.Events.OrderBy(a => a.Date).ToListAsync();
        }

        // GET: api/appointments/GetClientAppointments/{clientId}
        [HttpGet("GetClientAppointments/{clientId}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetClientAppointments(int clientId)
        {
            var appointments = await _context.Events
                .Where(e => e.Client_Id == clientId)
                .ToListAsync();

            if (appointments == null || !appointments.Any())
                return NotFound("No appointments found for this client.");

            return Ok(appointments);
        }

        [HttpPost("BookAppointment")]
        public async Task<ActionResult<Event>> CreateAppointment(Event appointment)
        {
            // Validate the appointment first
            if (!DateTime.TryParse(appointment.Date, out var appointmentDate))
            {
                return BadRequest("Invalid date format");
            }

            if (!TimeSpan.TryParse(appointment.StartTime, out var startTime) ||
                !TimeSpan.TryParse(appointment.EndTime, out var endTime))
            {
                return BadRequest("Invalid time format");
            }

            _context.Events.Add(appointment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClientAppointments), new { clientId = appointment.Client_Id }, appointment);
        }

        // DELETE: api/appointments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Events.FindAsync(id);
            if (appointment == null) return NotFound();

            _context.Events.Remove(appointment);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("CheckAvailability/{socialWorkerId}")]
        public async Task<ActionResult> CheckAvailability(
            int socialWorkerId,
            [FromQuery] string selectedDay,
            [FromQuery] string startTime,
            [FromQuery] string endTime)
        {
            // Parse the selected day
            if (!DateTime.TryParse(selectedDay, out var selectedDate))
            {
                return BadRequest(new { message = "Invalid date format for selectedDay" });
            }

            // Parse start and end times
            if (!TimeSpan.TryParse(startTime, out var startTimeSpan) ||
                !TimeSpan.TryParse(endTime, out var endTimeSpan))
            {
                return BadRequest(new { message = "Invalid time format" });
            }

            var startDateTime = selectedDate.Date.Add(startTimeSpan);
            var endDateTime = selectedDate.Date.Add(endTimeSpan);

            if (endDateTime <= startDateTime)
            {
                return BadRequest(new { message = "End time must be later than start time" });
            }

            // Load all events for that social worker on that day
            var events = await _context.Events
                .Where(e => e.Social_Id == socialWorkerId && e.Date == selectedDay)
                .ToListAsync();

            bool hasConflict = false;

            foreach (var ev in events)
            {
                if (TimeSpan.TryParse(ev.StartTime, out var evStart) &&
                    TimeSpan.TryParse(ev.EndTime, out var evEnd))
                {
                    var evStartDateTime = selectedDate.Date.Add(evStart);
                    var evEndDateTime = selectedDate.Date.Add(evEnd);

                    if ((startDateTime >= evStartDateTime && startDateTime < evEndDateTime) ||
                        (endDateTime > evStartDateTime && endDateTime <= evEndDateTime) ||
                        (startDateTime <= evStartDateTime && endDateTime >= evEndDateTime))
                    {
                        hasConflict = true;
                        break;
                    }
                }
            }

            return Ok(new
            {
                available = !hasConflict,
                message = hasConflict ? "Time slot is not available" : "Time slot is available"
            });
        }
    }
}