using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using SoberPath_API.NewFolder;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagement(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpPost("UserLogin/{username}/{password}")]
        public async Task<ActionResult> Login(string username, string password)
        {

            var determin_array = new int[3];

            var find_user = await _context.Users.FirstOrDefaultAsync(client=>client.EmailAddress==username && client.Password==password);

            if(find_user==null)
            {
                return NotFound();
            }


            

            return Ok(new
            {
                user_id = find_user.Id,
                first_name = find_user.Name,
                firt_surname =find_user.Surname,
                type= find_user.GetType().Name,
            });

        }

        [HttpGet("GetClientBYID{Id}")]
        public async Task<ActionResult> GetClient(int Id)
        {
            var client = await _context.Clients.FindAsync(Id);
            if (client == null)
            {
                return NotFound();

            }

            return Ok(client);

        }


        [HttpPost("Register_Client")]
        public async Task<ActionResult<Client>> Register_Client(Client client)
        {
            if (client == null)
            {
                return BadRequest();
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = client.Id }, client);
        }

        [HttpGet("Get_SW_by_ID{Id}")]
        public async Task<ActionResult> GetSW(int Id)
        {
            var sw = await _context.Social_Workers.FindAsync(Id);
            if (sw == null)
            {
                return NotFound();

            }

            return Ok(sw);

        }


        [HttpPost("Register_SW")]
        public async Task<ActionResult<Social_Worker>> Register_SW(Social_Worker sw)
        {


            if (sw == null)
            {
                return BadRequest();
            }

            _context.Social_Workers.Add(sw);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClient), new { id = sw.Id }, sw);



        }

        [HttpGet("GetClients")]
        public async Task<ActionResult<Client>> GetClients()
        {
            var clients = await _context.Clients.ToListAsync();
            if (clients == null)
            {
                return NotFound();
            }
            return Ok(clients);

        }


        [HttpGet("Get_NGO_Admin_by_ID{Id}")]
        public async Task<ActionResult> GetNGO_Admin(int Id)
        {
            var ngo_admin = await _context.NGO_Admins.FindAsync(Id);
            if (ngo_admin == null)
            {
                return NotFound();

            }

            return Ok(ngo_admin);

        }


        [HttpPost("Register_NGO_Admin")]
        public async Task<ActionResult<Client>> Register_NGO_Admin(NGO_Admin admin)
        {
            if (admin == null)
            {
                return BadRequest();
            }

            _context.NGO_Admins.Add(admin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNGO_Admin), new { id = admin.Id }, admin);
        }

        [HttpPost("Modify_ID")]

        public async Task<ActionResult<NGO_Admin>> ModifyID()
        {
            var last_user = await _context.Users.OrderBy(u=>u.Id).LastAsync();
            if(last_user==null)
            {
                return NotFound();
            }

            var last_ngo = await _context.NGO_Admins.OrderBy(u=>u.Id).LastAsync();
            if(last_ngo==null)
            {
                return NotFound();
            }

            last_ngo.Id = last_user.Id;

            await _context.SaveChangesAsync();
            return Ok(last_ngo);



        }


        [HttpGet("Get_Rehab_Admin_by_ID{Id}")]
        public async Task<ActionResult> GetRehab_Admin(int Id)
        {
            var rehab_admin = await _context.Rehab_Admins.FindAsync(Id);
            if (rehab_admin == null)
            {
                return NotFound();

            }

            return Ok(rehab_admin);

        }


        [HttpPost("Register_Rehab_Admin")]
        public async Task<ActionResult<Client>> Register_Rehab_Admin(Rehab_Admin admin)
        {
            if (admin == null)
            {
                return BadRequest();
            }

            _context.Rehab_Admins.Add(admin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRehab_Admin), new { id = admin.Id }, admin);
        }

        [HttpGet("Get_Substance")]

        public async Task<ActionResult<Substance>> GetSubstance(int id)
        {
            var substance= await _context.Substances.FindAsync(id);
            if(substance == null)
            {
                return NotFound();
            }

            return Ok(substance);
        }


        [HttpPost]
        public async Task<ActionResult<Substance>> Add_Substance(Substance substance)
        {
            if(substance == null)
            {
                return BadRequest();
            }

            _context.Substances.Add(substance);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSubstance),new { id = substance.Id },substance);
        }

        [HttpDelete("RemoveSubstance")]

        public async Task<ActionResult> RemoveSubstance(int id)
        {
            var sub = await _context.Substances.FindAsync(id);
            if (sub == null)
            {
                return NotFound();
            }

            _context.Substances.Remove(sub);
            await _context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPost("Edit_Substance")]

        public async Task<ActionResult<Substance>> Edit_Substance(int id, Substance substance)
        {
            var found_sub = await _context.Substances.FindAsync(substance.Id);
            if (found_sub == null)
            {
                return NotFound();
            }

            if (substance.Name != null)
            {
                found_sub.Name = substance.Name;
            }
            if (substance.Description != null)
            {
                found_sub.Description = substance.Description;
            }

            return Ok(found_sub);

        }

        [HttpGet("ForgotPassord/{value}")]

        public async Task<ActionResult<User>> ValidateEmail(string value)
        {
            var user = await _context.Users.Where(user => user.EmailAddress == value).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(); ;
            }

            EmailSender emailSender = new EmailSender();
            var username = user.Name + " " + user.Surname;
            await emailSender.Forgot_Password_Email("Password Recovery Email", "spha2729@gmail.com", username);

            return Ok();
        }


        [HttpPost("SetNewPassword/{value}/{newpassword}")]

        public async Task<ActionResult<User>> SetNewpassword(string value, string newpassword)
        {
            var foundemail = await _context.Users.Where(user => user.EmailAddress == value).FirstOrDefaultAsync();
            if (foundemail == null)
            {
                return NotFound(); ;
            }

            foundemail.Password = newpassword;
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
