using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Linq;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Socail_WorkerController(Sober_Context context) : ControllerBase
    {
        
        private readonly Sober_Context _context = context;

        [HttpGet("GetAssignedClients/{id}")]
        public async Task<ActionResult<Client>> GetAssignedClients(int id)
        {
            var clients = await _context.Clients
                .Where(cl => cl.Social_WorkerId == id)
                .OrderByDescending(cl => cl.Social_Worker_Assigned_Date)
                .ToListAsync();

            if (clients == null || !clients.Any())
            {
                return NotFound("No clients found for this social worker");
            }

            return Ok(clients);
        }

        [HttpGet("GetSessionsByUser/{userId}")]
        public async Task<ActionResult> GetSessionsByUser(int userId)
        {
            var sessions = await _context.Sessions
                .Where(cl => cl.ClientId == userId)
                .Select(cl => new
                {
                    sessionType = cl.Type,
                    Date = cl.Date,
                })
                .ToListAsync();

            if (sessions == null || !sessions.Any())
            {
                return NotFound();
            }

            return Ok(sessions);
        }

        [HttpPost("Schedule")]
        public async Task<ActionResult> Set_Schedule(Event booking)
        {
            if (booking == null)
            {
                return BadRequest("Booking data is required");
            }

            try
            {
                _context.Events.Add(booking);
                await _context.SaveChangesAsync();
                return Ok(booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Send_Application")]
        public async Task<ActionResult> Send_Application(Application application)
        {
            if(application==null)
            {  return BadRequest(); 
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return Ok(application);
        }





        [HttpGet("GetApplications/{id}")]
        public async Task<ActionResult> GetApplications(int id)
        {
            var applications = await (from app in _context.Applications
                                      join client in _context.Clients
                                      on app.ClientId equals client.Id
                                      where app.Social_WorkerId == id
                                      select new
                                      {
                                          ApplicationStatus = app.Status,
                                          ClientId = client.Id,
                                          ClientName = client.Name,
                                          ClientSurname = client.Surname
                                      }).ToListAsync();

            if (applications == null || !applications.Any())
            {
                return NotFound();
            }

            return Ok(applications);
        }


       
        [HttpGet("upcoming-sessions/{socialWorkerId}")]
        public async Task<IActionResult> GetUpcomingSessions(int socialWorkerId)
        {
            if (socialWorkerId <= 0)
                return BadRequest("Invalid social worker ID.");

            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);

            var sessions = await _context.Events
                .Where(s => s.Social_Id == socialWorkerId
                            )
                .Select(
                       s => new
                       {
                           s.Id,
                           s.Client_Id,
                           ClientName = _context.Clients.Where(c => c.Id == s.Client_Id).Select(c => c.Name).FirstOrDefault(),
                           Assignment_Date = s.Date,
                           topic = s.Title

                       }).ToListAsync();
            return Ok(sessions);


        }

        [HttpGet("search-by-socialworker/{socialWorkerId}")]
        public async Task<IActionResult> SearchClientsBySocialWorker(int socialWorkerId, [FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty.");

            searchTerm = searchTerm.Trim().ToLower();

            var clients = await _context.Events
                .Where(sb => sb.Social_Id == socialWorkerId)
                .Join(
                    _context.Clients,
                    sb => sb.Client_Id,
                    c => c.Id,
                    (sb, c) => c
                )
                .Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Surname) && c.Surname.ToLower().Contains(searchTerm))
                )
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Surname,
                    
                })
                .Distinct() // avoid duplicates if a client has multiple bookings
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(clients);
        }
    }
}

