using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;
        [HttpGet("GetSessions")]
        public async Task<ActionResult<Session>> GetSessions()
        {
            var Session_ = await _context.Sessions.ToListAsync();
            if (Session_ == null)
            {
                return NotFound();
            }
            return Ok(Session_);
        }

        [HttpGet("GetSession_by{Id}")]
        public async Task<ActionResult<Session>> GetSession_ByID(int Id)
        {
            var Social_Worker_ = await _context.Social_Workers.FindAsync(Id);
            if (Social_Worker_ == null)
            {
                return NotFound();
            }

            return Ok(Social_Worker_);
        }

        [HttpGet("GetSessionsByUser/{userId}")]
        public async Task<ActionResult> GetSessionsByUser(int userId)
        {
            var sessions = await _context.Sessions
                .Where(cl => cl.ClientId == userId)
                .Select(cl => new
                {
                    SessionType = cl.Type,
                    Date = cl.Date,
                    session_Note = cl.Session_Note
                })
                .ToListAsync();

            if (sessions == null || !sessions.Any())
            {
                return NotFound(new { message = "No sessions found for this user." });
            }

            return Ok(sessions);
        }

        [HttpPost("Add_Session{clientId}/{sw_id }")]
        public async Task<ActionResult<Session>> Add_Session_Admin(int clientId,int sw_id,Session session)
        {
            if (session == null)
            {
                return BadRequest();
            }

            var client = await _context.Clients.FindAsync(clientId);
            var sw= await _context.Social_Workers.FindAsync(sw_id);
            if(client == null || sw==null)
            {
                return NotFound();
            }
            
            session.ClientId=clientId;
            session.Social_WorkerId=sw_id;
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSession_ByID), session);
        }

        [HttpPut("EditSession{Id}")]
        public async Task<ActionResult<Session>> Edit_Session_Admin(int Id, Session new_session)
        {
            var Session_ = await _context.Sessions.FindAsync(Id);
            if (Session_ == null)
            {
                return NotFound();
            }
            if(new_session.Duration!=null)
            {
                Session_.Duration = new_session.Duration;
            }
            
            if (new_session.Date!=null)
            {
                Session_.Date = new_session.Date;
            }
            
            if (new_session.Duration!=null)
            {
                Session_.Duration = new_session.Duration;
            }
            

            
            


            await _context.SaveChangesAsync();
            return Ok(Session_);
        }
        
        [HttpDelete("Remove_Session{Id}")]
        public async Task<ActionResult<Session>> Remove_Session_ByID(int id)
        {
            var Session_ = await _context.Sessions.FindAsync(id);
            if (Session_ == null)
            {
                return NotFound();
            }

            _context.Sessions.Remove(Session_);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("Post_Session")]
        public async Task<ActionResult> Post_Session(Session session)
        {
            if (session == null)
            {
                return BadRequest();
            }

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();
            return Ok(session);
        }


    }
}

