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
    public class ApplicationController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpGet("GetAllApplications")]
        public async Task<ActionResult> GetAllApplications()
        {
            var applications = await _context.Applications.OrderByDescending(app => app.Id).Where(app => app.Status != null && app.ClientId > 0 && app.ClientId != null).Select(app => new
            {

                status = app.Status,
                id = app.Id,
                clientid = app.ClientId,
                name = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Name).FirstOrDefault(),
                surname = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Surname).FirstOrDefault(),

            }).ToListAsync();

            if (applications == null || !applications.Any())
            {
                return NotFound("No applications found.");
            }

            return Ok(applications);
        }

        [HttpGet("GetSentApplications")]
        public async Task<ActionResult> GetSentApplications()
        {
            var anonymous = await _context.Applications.Where(application => application.Status != null).Select(application => new
            {
                status = application.Status,
                clientid = _context.Clients.Where(cl => application.ClientId == cl.Id).Select(cl => cl.Id).FirstOrDefault(),
                clientname = _context.Clients.Where(cl => application.ClientId == cl.Id).Select(cl => cl.Name).FirstOrDefault(),
                clientsurname = _context.Clients.Where(cl => application.ClientId == cl.Id).Select(cl => cl.Surname).FirstOrDefault(),


            }).ToListAsync();

            if (anonymous == null)
            {
                return NotFound("Applications not found");
            }

            return Ok(anonymous);
        }

        [HttpGet("GetApprovedApplications")]
        public async Task<ActionResult> GetApproved()
        {

            var returnval = await _context.Applications.Where(app => app.Status != null && app.Status == "Approved & Allocated" && app.ClientId != null).Select(app => new
            {
                id = app.ClientId,
                Name = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Name).FirstOrDefault(),
                surname = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Surname).FirstOrDefault(),
                id_number = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.ID_Number).FirstOrDefault(),
                Gender = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Gender).FirstOrDefault(),
                Nok_Name = _context.Next_Of_Kins.Where(nok => nok.ClientId == app.ClientId).Select(nok => nok.Name).FirstOrDefault(),
                Nok_Surname = _context.Next_Of_Kins.Where(nok => nok.ClientId == app.ClientId).Select(nok => nok.Surname).FirstOrDefault(),
                Nok_phone = _context.Next_Of_Kins.Where(Nok => Nok.ClientId == app.ClientId).Select(nok => nok.Phone_number).FirstOrDefault()
            }).ToArrayAsync();


            return Ok(returnval);
        }

        [HttpGet("GetApplicationData/{id}")]
        public async Task<ActionResult> GetApplocationData(int id)
        {
            var application = await _context.Applications.Where(app => app.ClientId == id).FirstOrDefaultAsync();
            if (application == null)
            {
                return NotFound();

            }

            var returnval = new
            {
                id = application.Id,
                applicationDate = application.Date,
                editableReason = application.RejectionReason,
                summary = application.Summary,
                substances = _context.Substances.Where(sub => sub.ClientId == id).Select(sub => sub.Name).ToList(),
                socialWorkerName = _context.Social_Workers.Where(sw => sw.Id == application.Social_WorkerId).Select(sw => sw.Name).FirstOrDefault(),
                fileName = application.FileName,
                content = application.Data
            };

            return Ok(returnval);
        }

        [HttpGet("Pending_Applications")]
        public async Task<ActionResult> Get_pendingapplication()
        {
            var returnval = await _context.Applications.Where(app => app.ClientId != null && app.Status == "Pending").CountAsync();
            return Ok(returnval);
        }

        [HttpPost("CreateApplication")]
        public async Task<IActionResult> CreateApplication([FromForm] IFormFile? file,
         [FromForm] string? date,
         [FromForm] string? summary,
         [FromForm] int? clientId,
         [FromForm] int? social_workerId)
        {
            var application = new Application
            {
                Date = date,
                Summary = summary,
                ClientId = clientId,
                Social_WorkerId = social_workerId,
                Status = "Pending",
                HasRelapse = false
            };

            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                application.FileName = file.FileName;
                application.ContentType = file.ContentType;
                application.Data = ms.ToArray();
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new { application.Id, application.FileName, application.Status });
        }


        [HttpGet("DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null || application.Data == null)
                return NotFound("File not found");

            return File(application.Data, application.ContentType ?? "application/octet-stream", application.FileName);
        }

    }

}
