using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SoberPath_API.Context;
using SoberPath_API.Models;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Rehab_AdminController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpGet("GetRehab_Admins")]
        public async Task<ActionResult<Rehab_Admin>> GetRehab_Admins()
        {
            var rehab_Admins = await _context.Rehab_Admins.Select(r => new { r.Id, r.Name, r.Surname, r.Address, r.EmailAddress, r.Phone_Number }).ToListAsync();
            if (rehab_Admins == null)
            {
                return NotFound();
            }
            return Ok(rehab_Admins);
        }

        [HttpGet("GetRehab_Admin_ById/{id}")]
        public async Task<ActionResult<NGO_Admin>> GetRehab_AdminById(int id)
        {
            var rehab_Admin = await _context.Rehab_Admins.FindAsync(id);
            if (rehab_Admin == null)
            {
                return NotFound();

            }
            return Ok(rehab_Admin);
        }

        [HttpPost("UpdateApplicationStatus/{applicationId}/{newStatus}")]
        public async Task<ActionResult> UpdateApplicationStatus(int applicationId, string newStatus)
        {
            var application = await _context.Applications.FindAsync(applicationId);

            if (application == null)
            {
                return NotFound();
            }

            application.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(new { applicationId, newStatus });

        }



        [HttpPost("Add_Rehab_Admin")]
        public async Task<ActionResult<Rehab_Admin>> Add_Rehab_Admin(Rehab_Admin rehab_Admin)
        {
            if (rehab_Admin == null)
            {
                return BadRequest();
            }
            _context.Rehab_Admins.Add(rehab_Admin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRehab_AdminById), new { id = rehab_Admin.Id }, rehab_Admin);
        }
        [HttpPost("Edit_Rehab_Admin")]

        public async Task<ActionResult<Rehab_Admin>> Edit_Rehab_Admin(int id, Rehab_Admin newAdmin)
        {
            if (id > 0 && newAdmin != null)
            {
                var found_admin = await _context.Clients.FindAsync(id);
                if (found_admin != null)
                {
                    if (newAdmin.Name != null)
                    {
                        found_admin.Name = newAdmin.Name;

                    }
                    if (newAdmin.Surname != null)
                    {
                        found_admin.Surname = newAdmin.Surname;
                    }

                    if (newAdmin.Race != null)
                    {
                        found_admin.Race = newAdmin.Race;
                    }

                    if (newAdmin.Gender != null)
                    {
                        found_admin.Gender = newAdmin.Gender;
                    }
                    if (newAdmin.Address != null)
                    {
                        found_admin.Address = newAdmin.Address;

                    }

                    if (newAdmin.Phone_Number != null)
                    {
                        found_admin.Phone_Number = newAdmin.Phone_Number;
                    }

                    if (newAdmin.EmailAddress != null)
                    {
                        found_admin.EmailAddress = newAdmin.EmailAddress;

                    }
                    if (newAdmin.Password != null)
                    {
                        found_admin.Password = newAdmin.Password;

                    }

                }
                else
                {
                    BadRequest("Rehab_Admin not found");
                }

            }
            else
            {
                BadRequest("ID and Rehab_Admin objects are null");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("Remove_Rehab_Admin_by")]
        public async Task<ActionResult> Remove_Rehab_Admin(int id)
        {
            var findadmin = await _context.Rehab_Admins.FindAsync(id);
            if (findadmin == null)
            {
                return NotFound();

            }

            _context.Rehab_Admins.Remove(findadmin);
            await _context.SaveChangesAsync();
            return Ok("Client Removed Successfully");


        }

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
                Nok_phone = _context.Next_Of_Kins.Where(Nok => Nok.ClientId == app.ClientId).Select(nok => nok.Phone_number).FirstOrDefault()
            }).ToArrayAsync();


            return Ok(returnval);
        }


        [HttpPost("Record_progress")]

        public async Task<ActionResult> RecordProgress(Rehabilitation_Progress progress)
        {
            if (progress == null)
            {
                return BadRequest();
            }

            _context.Rehabilitation_Progresses.Add(progress);
            await _context.SaveChangesAsync();

            return NoContent();

        }


        [HttpPost("Discharge/{id}/{reason}")]

        public async Task<ActionResult> Discharge(int id, string reason)
        {
            var application = await _context.Applications.Where(app => app.Status != null && app.Status.Equals("Approved & Allocated") && app.ClientId == id).FirstOrDefaultAsync();
            if (application == null)
            {
                return NotFound();
            }

            application.Status = "Discharged";
            application.Discharge_Date = DateTime.Now.Date.ToString();
            application.Discharge_Reason = reason;

            var room_details = await _context.rooms.Where(r => r.ClientId == application.ClientId).FirstOrDefaultAsync();
            if (room_details != null)
            {
                _context.rooms.Remove(room_details);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost("AllocaeRoom")]

        public async Task<ActionResult> AllocateRoom(Room roomdetails)
        {
            if (roomdetails == null)
            {
                return BadRequest();

            }
            else if (roomdetails != null)
            {
                var findroom = _context.rooms.Where(r => r.ClientId == roomdetails.ClientId).FirstOrDefault();
                if (findroom != null)
                {
                    return BadRequest("Room already occupied");
                }
                var findApplication = _context.Applications.Where(app => app.ClientId == roomdetails.ClientId).FirstOrDefault();
                if (findApplication == null)
                {
                    return NotFound();
                }
                findApplication.Status = "Approved & Allocated";

                _context.rooms.Add(roomdetails);
            }


            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet("Get_OccupiedRooms/{buildingName}")]

        public async Task<ActionResult> GetOcuupiedRooms(string buildingName)
        {
            var rooms = await _context.rooms.Where(room => room.ClientId != null && room != null && room.BuildingName == buildingName).Select(room => new
            {
                buildingname = buildingName,
                id = room.RoomNumber,
                name = "Room " + room.RoomNumber,
                isAllocated = true,
                allocatedToClientName = _context.Clients.Where(cl => cl.Id == room.ClientId).Select(cl => cl.Name).FirstOrDefault(),
                allocatedToClientSurname = _context.Clients.Where(cl => cl.Id == room.ClientId).Select(cl => cl.Surname).FirstOrDefault(),
                allocatedToClientId = room.ClientId,
                allocationDate = room.AllocatedDate,

            }).ToListAsync();
            if (rooms == null)
            {
                return NotFound();

            }

            return Ok(rooms);

        }


        [HttpPost("SetAllTOPENDING")]

        public async Task<ActionResult> SetAllToPending()
        {
            var list = await _context.Applications.ToListAsync();
            if (list == null)
            {
                return BadRequest();
            }

            for (var i = 0; i < list.Count(); i++)
            {
                list[i].Status = "Pending";

            }

            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpPost("Nullify")]

        public async Task<ActionResult> NUllify()
        {
            var list = await _context.Applications.ToListAsync();
            if (list == null)
            {
                return BadRequest();
            }

            for (var i = 0; i < list.Count(); i++)
            {
                list[i].ClientId = null;

            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("SW_List")]
        public async Task<ActionResult> GetSWList()
        {
            var returnval = await _context.Social_Workers.Select(sw => new
            {
                sw.Id,
                sw.Name,
                sw.Surname,

            }).ToListAsync();

            if (returnval == null || !returnval.Any())
            {
                return NotFound();
            }

            return Ok(returnval);
        }


    }
}
