using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Hobs;
using SoberPath_API.Models;
using System.Globalization;

namespace SoberPath_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly Sober_Context _context;
        private readonly IHubContext<Rehab_NotificationHub> _hubContext;

        public EventController(Sober_Context context, IHubContext<Rehab_NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpGet("GetUpcomingSessions/{social_Id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUpcomingSessions(int social_Id)
        {
            try
            {
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);
                var todayString = today.ToString("yyyy-MM-dd");
                var nextWeekString = nextWeek.ToString("yyyy-MM-dd");

                // Get events for the social worker within the date range
                var events = await _context.Events
                    .Where(e => e.Social_Id == social_Id &&
                                string.Compare(e.Date, todayString) >= 0 &&
                                string.Compare(e.Date, nextWeekString) <= 0)
                    .ToListAsync();

                var sessions = events
                    .Select(e => new
                    {
                        id = e.Id,
                        client_Id = e.Client_Id,
                        clientName = e.Client_Id.HasValue && e.Client_Id.Value > 0
                            ? _context.Clients
                                .Where(c => c.Id == e.Client_Id.Value)
                                .Select(c => c.Name)
                                .FirstOrDefault() ?? "Client Not Found"
                            : "No Client Assigned",
                        date = e.Date,
                        startTime = e.StartTime,
                        endTime = e.EndTime,
                        topic = e.Title ?? "No Title",
                        hasClient = e.Client_Id.HasValue && e.Client_Id.Value > 0
                    })
                    .OrderBy(e => e.date) // Already in yyyy-MM-dd format, so string comparison works
                    .ThenBy(e => e.startTime)
                    .ToList();

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("HasClient/{eventId}")]
        public async Task<ActionResult<bool>> HasClient(int eventId)
        {
            try
            {
                var eventEntity = await _context.Events
                    .FirstOrDefaultAsync(e => e.Id == eventId);

                if (eventEntity == null)
                    return NotFound($"Event with ID {eventId} not found.");

                // Check for both null AND 0 (or other invalid values)
                var hasClient = eventEntity.Client_Id.HasValue && eventEntity.Client_Id.Value > 0;

                return Ok(hasClient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetMonthlyEvents/{socialId}/{month}/{year}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByMonth(int socialId, int month, int year)
        {
            try
            {
                var startDate = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
                var endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

                var events = await _context.Events
                    .Where(e => e.Social_Id == socialId &&
                           string.Compare(e.Date, startDate) >= 0 &&
                           string.Compare(e.Date, endDate) <= 0)
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetDayEvents/{socialId}/{date}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByDate(int socialId, string date)
        {
            try
            {
                // Validate date format
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
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
            try
            {
                if (calendarEvent.Social_Id == null || calendarEvent.Social_Id <= 0)
                {
                    return BadRequest("Valid Social Worker ID is required");
                }

                if (!DateTime.TryParseExact(calendarEvent.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd");
                }

                if (!TimeSpan.TryParse(calendarEvent.StartTime, out TimeSpan startTime) ||
                    !TimeSpan.TryParse(calendarEvent.EndTime, out TimeSpan endTime))
                {
                    return BadRequest("Invalid time format. Use HH:mm");
                }

                if (startTime >= endTime)
                {
                    return BadRequest("Start time must be before end time");
                }

                _context.Events.Add(calendarEvent);
                await _context.SaveChangesAsync();

                // 🔔 Notify the client if assigned
                if (calendarEvent.Client_Id.HasValue)
                {
                    var client = await _context.Clients.FindAsync(calendarEvent.Client_Id.Value);
                    if (client != null)
                    {
                        await _hubContext.Clients.All.SendAsync(
                            "ReceiveNotification",
                            new
                            {
                                ClientId = client.Id,
                                Message = $"New event scheduled on {calendarEvent.Date} from {calendarEvent.StartTime} to {calendarEvent.EndTime}",
                                EventId = calendarEvent.Id
                            }
                        );
                    }
                }

                return CreatedAtAction(nameof(GetEventById), new { id = calendarEvent.Id }, calendarEvent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetEvent/{id}")]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            try
            {
                var eventEntity = await _context.Events.FindAsync(id);
                if (eventEntity == null)
                {
                    return NotFound();
                }
                return Ok(eventEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, Event updatedEvent)
        {
            try
            {
                if (id != updatedEvent.Id)
                {
                    return BadRequest("Event ID mismatch");
                }

                var existingEvent = await _context.Events.FindAsync(id);
                if (existingEvent == null)
                {
                    return NotFound();
                }

                // Check availability excluding the current event
                var availabilityRequest = new EventAvailabilityRequest
                {
                    SocialWorkerId = updatedEvent.Social_Id.Value,
                    Date = updatedEvent.Date,
                    StartTime = updatedEvent.StartTime,
                    EndTime = updatedEvent.EndTime,
                    ExcludeEventId = id
                };

                var availabilityResult = await CheckEventAvailability(availabilityRequest);
                if (availabilityResult.Result is OkObjectResult okResult &&
                    okResult.Value is AvailabilityResponse response &&
                    !response.IsAvailable)
                {
                    return Conflict(new
                    {
                        Message = response.Message,
                        ConflictingEventId = response.ConflictingEventId
                    });
                }

                // Update event properties
                existingEvent.Title = updatedEvent.Title;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.StartTime = updatedEvent.StartTime;
                existingEvent.EndTime = updatedEvent.EndTime;
                existingEvent.Client_Id = updatedEvent.Client_Id;
                existingEvent.NGO_Id = updatedEvent.NGO_Id;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetClientEvents/{clientId}")]
        public async Task<ActionResult> GetClientEvents(int clientId)
        {
            try
            {
                var events = await _context.Events
                    .Where(e => e.Client_Id == clientId)
                    .OrderBy(e => e.Date)
                    .ThenBy(e => e.StartTime)
                    .ToListAsync();

                if (!events.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No sessions found for this client.",
                        data = new List<object>()
                    });
                }

                var clientEvents = events.Select(e => new
                {
                    id = e.Id, // match Notification.id
                    title = e.Title ?? "No Topic",
                    message = e.Description ?? "No Description",
                    createdDate = e.Date + " - " + (e.StartTime),
                    isRead = e.IsRead ?? false,
                    relatedClientId = e.Client_Id,
                    clientName = _context.Clients
                        .Where(c => c.Id == e.Client_Id)
                        .Select(c => c.Name + " " + c.Surname)
                        .FirstOrDefault() ?? "Unknown Client",
                    socialWorkerName = e.Social_Id.HasValue
                        ? _context.Social_Workers
                            .Where(s => s.Id == e.Social_Id.Value)
                            .Select(s => s.Name + " " + s.Surname)
                            .FirstOrDefault() ?? "Not Assigned"
                        : "Not Assigned",
                    type = "event" // ADD THIS LINE - matches frontend expectation
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Client events retrieved successfully",
                    data = clientEvents
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Internal server error: {ex.Message}",
                    data = new List<object>()
                });
            }
        }

        [HttpPost("CheckAvailability")]
        public async Task<ActionResult<AvailabilityResponse>> CheckEventAvailability(EventAvailabilityRequest request)
        {
            try
            {
                // Validate input
                if (!DateTime.TryParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eventDate))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd");
                }

                if (!TimeSpan.TryParse(request.StartTime, out TimeSpan startTime) ||
                    !TimeSpan.TryParse(request.EndTime, out TimeSpan endTime))
                {
                    return BadRequest("Invalid time format. Use HH:mm");
                }

                if (startTime >= endTime)
                {
                    return BadRequest("Start time must be before end time");
                }

                // Check for overlapping events for the same social worker
                var query = _context.Events
                    .Where(e => e.Social_Id == request.SocialWorkerId &&
                               e.Date == request.Date);

                if (request.ExcludeEventId.HasValue)
                {
                    query = query.Where(e => e.Id != request.ExcludeEventId.Value);
                }

                var overlappingEvents = await query.ToListAsync();

                foreach (var existingEvent in overlappingEvents)
                {
                    if (!TimeSpan.TryParse(existingEvent.StartTime, out TimeSpan existingStart) ||
                        !TimeSpan.TryParse(existingEvent.EndTime, out TimeSpan existingEnd))
                    {
                        continue; // Skip invalid time formats
                    }

                    // Check for time overlap
                    if (DoTimeSlotsOverlap(startTime, endTime, existingStart, existingEnd))
                    {
                        return Ok(new AvailabilityResponse
                        {
                            IsAvailable = false,
                            Message = $"Time slot conflicts with existing event: {existingEvent.Title}",
                            ConflictingEventId = existingEvent.Id,
                            ConflictingEventTitle = existingEvent.Title
                        });
                    }
                }

                return Ok(new AvailabilityResponse
                {
                    IsAvailable = true,
                    Message = "Time slot is available"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private bool DoTimeSlotsOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && end1 > start2;
        }
    }

    public class EventAvailabilityRequest
    {
        public int SocialWorkerId { get; set; }
        public string Date { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int? ExcludeEventId { get; set; }
    }

    public class AvailabilityResponse
    {
        public bool IsAvailable { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ConflictingEventId { get; set; }
        public string? ConflictingEventTitle { get; set; }
    }

    public class SessionRecording
    {
        public string Notes { get; set; } = string.Empty;
    }

    public class SessionRecord
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public DateTime RecordedAt { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}