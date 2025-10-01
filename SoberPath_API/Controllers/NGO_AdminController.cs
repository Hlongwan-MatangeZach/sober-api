using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Net;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NGO_AdminController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpGet("GetNGO_Admins")]
        public async Task<ActionResult<Client>> GetNGO_Admins()
        {
            var ngo_admins = await _context.NGO_Admins.Select(n=> new {n.Id,
                n.Name,n.Surname,
                n.Address,
                n.EmailAddress,
                n.Phone_Number }).ToListAsync();
            if (ngo_admins == null)
            {
                return NotFound();
            }

            
            return Ok(ngo_admins);
        }

        [HttpGet("GetNGO_Admin_ById/{id}")]
        public async Task<ActionResult<NGO_Admin>> GetNGO_AdminById(int id)
        {
            var ngo_admin = await _context.NGO_Admins.FindAsync(id);
            if (ngo_admin == null)
            {
                return NotFound();
            }
            return Ok(ngo_admin);
        }

        [HttpPost("Add_NGO_Admin")]
        public async Task<ActionResult<NGO_Admin>> AddNGO_Admin(NGO_Admin ngo_admin)
        {
            if (ngo_admin == null)
            {
                return BadRequest();
            }
            _context.NGO_Admins.Add(ngo_admin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNGO_AdminById), new { id = ngo_admin.Id }, ngo_admin);
        }
        [HttpPost("Edit_NGO_Admin/{id}")]
        public async Task<ActionResult<NGO_Admin>> EditNGOAdmin(int id, NGO_Admin newAdmin)
        {
            var found_admin = await _context.NGO_Admins.FindAsync(id);
            if (id > 0 && newAdmin != null)
            {
                
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
                    BadRequest("NGO_Admin not found");
                }

            }
            else
            {
                BadRequest("ID and NGO_Admin objects are null");
            }

            await _context.SaveChangesAsync();
            return Ok(found_admin);
        }

        [HttpDelete("Remove_NGO_Admin_by")]
        public async Task<ActionResult> Remove_NGO_Admin(int id)
        {
            var findadmin = await _context.NGO_Admins.FindAsync(id);
            if (findadmin == null)
            {
                return NotFound();

            }

            _context.NGO_Admins.Remove(findadmin);
            await _context.SaveChangesAsync();
            return Ok("Client Removed Successfully");


        }

        [HttpPost("EditClient/{id}")]
        public async Task<ActionResult<Client>> EditClient(int id, Client newClient)
        {
            var found_client = await _context.Clients.FindAsync(id);
            if (id > 0 && newClient != null)
            {
                if (found_client != null)
                {
                    if (newClient.Name != null)
                    {
                        found_client.Name = newClient.Name;
                    }
                    if (newClient.Surname != null)
                    {
                        found_client.Surname = newClient.Surname;
                    }

                    if(newClient.IsRead!=null)
                    {
                        found_client.IsRead = newClient.IsRead;
                    }

                    if (newClient.Race != null)
                    {
                        found_client.Race = newClient.Race;
                    }

                    if (newClient.Gender != null)
                    {
                        found_client.Gender = newClient.Gender;
                    }
                    if (newClient.Address != null)
                    {
                        found_client.Address = newClient.Address;
                    }

                    if (newClient.ID_Number != null)
                    {
                        found_client.ID_Number = newClient.ID_Number;
                    }

                    if (newClient.Phone_Number != null)
                    {
                        found_client.Phone_Number = newClient.Phone_Number;
                    }

                    if (newClient.EmailAddress != null)
                    {
                        found_client.EmailAddress = newClient.EmailAddress;
                    }
                    if (newClient.Password != null)
                    {
                        found_client.Password = newClient.Password;
                    }

                    if (newClient.Social_WorkerId != null)
                    {
                        // Check if social worker is actually being changed
                        if (found_client.Social_WorkerId != newClient.Social_WorkerId)
                        {
                            found_client.Social_WorkerId = newClient.Social_WorkerId;
                            found_client.Social_Worker_Assigned_Date = DateTime.Now;
                        }
                        else
                        {
                            // Social worker ID is the same, just update without changing date
                            found_client.Social_WorkerId = newClient.Social_WorkerId;
                        }
                    }
                    // Note: If newClient.Social_WorkerId is null, we don't change the existing social worker ID
                }
                else
                {
                    return BadRequest("Client not found");
                }
            }
            else
            {
                return BadRequest("ID and Client objects are null");
            }

            await _context.SaveChangesAsync();
            return NoContent();
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

        [HttpGet("Get_SW_list")]
        public async Task<ActionResult<IEnumerable<Social_Worker>>> Get_SW_list()
        {
            var sw = await _context.Social_Workers.Select(s => new {
                s.Id,
                s.Name,
                s.Surname,
                s.EmailAddress,
                s.Phone_Number,
                s.Address,


            }).ToListAsync();
            if (sw == null || !sw.Any())
            {
                return NotFound();
            }


            return Ok(sw);
        }

        [HttpGet("Get_SW_ById/{id}")]
        public async Task<ActionResult<Social_Worker>> Get_SW_By(int id)
        {
            var sw = await _context.Social_Workers.FindAsync(id);
            if (sw == null)
            {
                return NotFound();
            }
            return Ok(sw);
        }

        [HttpPost("Add_SW")]
        public async Task<ActionResult<Client>> Add_SW(Social_Worker sw)
        {
            if (sw == null)
            {
                return BadRequest();
            }
            _context.Social_Workers.Add(sw);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get_SW_By), new { id = sw.Id }, sw);
        }

        [HttpPost("EditSW/{id}")]
        public async Task<ActionResult<Social_Worker>> EditClient(int id, Social_Worker social_Worker)
        {
            var found_sw = await _context.Social_Workers.FindAsync(id);
            if (id > 0 && social_Worker != null)
            {

                if (found_sw != null)
                {
                    if (social_Worker.Name != null)
                    {
                        found_sw.Name = social_Worker.Name;

                    }
                    if (social_Worker.Surname != null)
                    {
                        found_sw.Surname = social_Worker.Surname;
                    }

                    if (social_Worker.Race != null)
                    {
                        found_sw.Race = social_Worker.Race;
                    }

                    if (social_Worker.Gender != null)
                    {
                        found_sw.Gender = social_Worker.Gender;
                    }
                    if (social_Worker.Address != null)
                    {
                        found_sw.Address = social_Worker.Address;

                    }

                    if (social_Worker.Phone_Number != null)
                    {
                        found_sw.Phone_Number = social_Worker.Phone_Number;
                    }

                    if (social_Worker.EmailAddress != null)
                    {
                        found_sw.EmailAddress = social_Worker.EmailAddress;

                    }
                    if (social_Worker.Password != null)
                    {
                        found_sw.Password = social_Worker.Password;

                    }


                }
                else
                {
                    BadRequest("Social_Worker not found");
                }

            }
            else
            {
                BadRequest("ID and Social_Worker objects are null");
            }

            await _context.SaveChangesAsync();
            return Ok(found_sw);
        }

        [HttpDelete("Remove_SW_by/{id}")]
        public async Task<ActionResult> Remove_SW(int id)
        {
            // Load the Social_Worker with ALL related collections
            var socialWorker = await _context.Social_Workers
                .Include(sw => sw.Applications)
                .Include(sw => sw.Client_Assignments)
                .Include(sw => sw.Sessions)
                .Include(sw => sw.Social_Worker_Schedules)
                .Include(sw => sw.Clients)
                .FirstOrDefaultAsync(sw => sw.Id == id);

            if (socialWorker == null)
            {
                return NotFound();
            }

            // Remove all dependent records
            _context.Applications.RemoveRange(socialWorker.Applications!);
            _context.ClientAssignments.RemoveRange(socialWorker.Client_Assignments!);
            _context.Sessions.RemoveRange(socialWorker.Sessions!);
            _context.Social_Worker_Schedules.RemoveRange(socialWorker.Social_Worker_Schedules!);

            // If Clients should NOT be deleted (just unassigned), set their Social_WorkerId to null
            foreach (var client in socialWorker.Clients!)
            {
                client.Social_WorkerId = null; // Unassign clients instead of deleting them
            }

            // Finally, remove the Social_Worker
            _context.Social_Workers.Remove(socialWorker);

            await _context.SaveChangesAsync();
            return Ok("Social Worker Removed Successfully");
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<NGO_Center>>> GetAllNgos()
        {
            try
            {
                var ngos = await _context.NGOs.ToListAsync();
                if (!ngos.Any())
                    return NotFound("No NGOs found.");
                return Ok(ngos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Nearest")]
        public async Task<ActionResult<IEnumerable<object>>> GetNearestNgos(double lat, double lng, int count = 5)
        {
            try
            {
                var ngos = await _context.NGOs.ToListAsync();
                if (!ngos.Any())
                    return NotFound("No NGOs found.");

                var nearest = ngos
                    .Select(n => new
                    {
                        n.Id,
                        n.Name,
                        n.Description,
                        n.Rating,
                        n.Type,
                        n.Latitude,
                        n.Longitude,
                        n.Address,
                        n.IsRecommended,
                        Distance = GetDistance(lat, lng, n.Latitude, n.Longitude)
                    })
                    .OrderBy(n => n.Distance)
                    .Take(count)
                    .ToList();

                return Ok(nearest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Helper: Haversine Formula (distance in KM)
        private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Radius of earth in KM
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double angle) => Math.PI * angle / 180.0;
    }


}

