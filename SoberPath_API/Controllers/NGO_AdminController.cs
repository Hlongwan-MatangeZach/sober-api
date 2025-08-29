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
                        found_client.Social_WorkerId = newClient.Social_WorkerId;
                    }
                }
                else
                {
                    BadRequest("Client not found");
                }

            }
            else
            {
                BadRequest("ID and Client objects are null");
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}

