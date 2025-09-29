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
    public class ClientController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;


        [HttpGet("GetClients")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            var clients = await _context.Clients.Select(c => new { c.Id,
                c.Name,
                c.Surname,
                c.Gender,
                ID_Number = c.ID_Number,
                noK_Name = _context.Next_Of_Kins.Where(nk => nk.ClientId == c.Id).Select(nk => nk.Name).FirstOrDefault(),
                noK_Phone = _context.Next_Of_Kins.Where(nk => nk.ClientId == c.Id).Select(nk => nk.Phone_number).FirstOrDefault(),
                assigned_SW = _context.Social_Workers.Where(sw => sw.Id == c.Social_WorkerId).Select(sw => sw.Name).FirstOrDefault(),
            }).ToListAsync();
            if (clients == null || !clients.Any())
            {
                return NotFound();
            }

            return Ok(clients);
        }

        [HttpGet("GetClientById/{id}")]
        public async Task<ActionResult<Client>> GetClientById(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client);
        }

        [HttpPost("AddClient")]
        public async Task<ActionResult<Client>> AddClient(Client client)
        {
            if (client == null)
            {
                return BadRequest();
            }
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();

           // EmailSender emailSender = new EmailSender();
            //var username = client.Name + " " + client.Surname;
           // await emailSender.Client_Registration_Email("Sucessful Registration Confirmation", "sizwegazide4@gmail.com", username);
            return CreatedAtAction(nameof(GetClientById), new { id = client.Id }, client);
        }

        [HttpGet("Assigned_SW_Name/{id}")]
        public async Task<ActionResult> GetAssined_SW_Name(int id)
        {
            var sw = await _context.Social_Workers.FindAsync(id);
            if (sw == null)
            {
                return NotFound();
            }

            return Ok(new { name = sw.Name });
        }

        [HttpGet("Get_CLient_NOK/{Id}")]
        public async Task<ActionResult<Next_of_Kin>> Get_Client_NOK(int Id)
        {
            var nok = await _context.Next_Of_Kins.Where(nok => nok.ClientId == Id).FirstOrDefaultAsync();
            if (nok == null)
            {
                return NotFound();
            }

            var return_nok = new { name = nok.Name, phone = nok.Phone_number };
            return Ok(return_nok);
        }

        [HttpDelete("DeleteClient/{Id}")]
        public async Task<ActionResult> DeleteClient(int Id)
        {
            // Load the client with ALL related collections
            var client = await _context.Clients
                .Include(c => c.Next_Of_Kins)
                .Include(c => c.Event)
                .Include(c => c.Sessions)
                .Include(c => c.Substances)
                .Include(c => c.Rehab_Admission)
                .Include(c => c.Client_Assignment)
                .Include(c => c.Application)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (client == null)
            {
                return NotFound();
            }

            // Remove all dependent records
            _context.Next_Of_Kins.RemoveRange(client.Next_Of_Kins);
            _context.Events.RemoveRange(client.Event);
            _context.Sessions.RemoveRange(client.Sessions);
            _context.Substances.RemoveRange(client.Substances);
            _context.Rehab_Admissions.RemoveRange(client.Rehab_Admission);
            _context.ClientAssignments.RemoveRange(client.Client_Assignment);
            _context.Applications.RemoveRange(client.Application);

            // Finally, remove the client
            _context.Clients.Remove(client);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("UpdateApplication/{clientId}/{newStatus}/{comment}/{rehab_id}")]
        public async Task<ActionResult> UpdateApplicationStatus(int clientId, string newStatus, string comment, int rehab_id)
        {

            var application = await _context.Applications.Where(app => app.ClientId == clientId).FirstOrDefaultAsync();
            if (application == null)
            {
                return NotFound();
            }


            application.Status = newStatus;
            application.RejectionReason = comment;
            application.Status_Update_Date = DateTime.Now.Date.ToString();
            application.IsRead = false;
            application.Rehab_AdminID = rehab_id;
            await _context.SaveChangesAsync();
            return NoContent();

        }

        [HttpGet("HasAppliedForRehab/{clientId}")]
        public async Task<ActionResult<bool>> HasAppliedForRehab(int clientId)
        {
            var hasApplied = await _context.Applications
                .AnyAsync(a => a.ClientId == clientId);

            return Ok(new { clientId, hasApplied });
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

        [HttpDelete("Remove_Client_by/{id}")]
        public async Task<ActionResult> RemoveClient(int id)
        {
            var findclient = await _context.Clients.FindAsync(id);
            if (findclient == null)
            {
                return NotFound();

            }

            _context.Clients.Remove(findclient);
            await _context.SaveChangesAsync();
            return Ok("Client Removed Successfully");


        }

        [HttpPost("Add_Substance")]
        public async Task<ActionResult<Substance>> AddSubstance(Substance substance)
        {
            if (substance == null)
            {
                return BadRequest();
            }
            _context.Substances.Add(substance);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClientById), new { id = substance.Id }, substance);
        }

        [HttpPost("Add_Next_of_Kin")]
        public async Task<ActionResult<Next_of_Kin>> AddNextOfKin(Next_of_Kin nextOfKin)
        {
            if (nextOfKin == null)
            {
                return BadRequest();
            }
            _context.Next_Of_Kins.Add(nextOfKin);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClientById), new { id = nextOfKin.Id }, nextOfKin);
        }

        [HttpPost("EditApplicationReason/{clientid}/{value}")]
        public async Task<ActionResult> EditApplicationStatus(int clientid, string value)
        {
            var application = await _context.Applications.Where(app => app.ClientId == clientid).FirstOrDefaultAsync();
            if (application == null)
            {
                return NotFound();
            }

         //   application.RehabReason = value;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("GetSocial_Worker_Id/{client_id}")]
        public async Task<ActionResult> Get_SW_Id(int client_id)
        {
            var client = await _context.Clients.FindAsync(client_id);
            if (client == null)
            {
                return NotFound();
            }
            return Ok(client.Social_WorkerId);
        }

        [HttpGet("GetClientAssignmentInfo/{clientId}")]
        public async Task<ActionResult> GetClientAssignmentInfo(int clientId)
        {
            var client = await _context.Clients
                .Where(c => c.Id == clientId && c.Social_WorkerId != null)
                .FirstOrDefaultAsync();

            if (client == null)
            {
                return NotFound("Client not found or not assigned to any social worker");
            }

            var socialWorker = await _context.Social_Workers
                .Where(sw => sw.Id == client.Social_WorkerId.Value)
                .Select(sw => new { sw.Name, sw.Surname })
                .FirstOrDefaultAsync();

            if (socialWorker == null)
            {
                return NotFound("Social worker not found");
            }

            return Ok(new
            {
                success = true,
                message = "Assignment info retrieved successfully",
                data = new[] {
        new {
            id = client.Id,
            title = "Social Worker Assignment",
            message = $"Client {client.Name} {client.Surname} assigned to Social Worker  {socialWorker.Name} {socialWorker.Surname}",
            createdDate = client.Social_Worker_Assigned_Date.Value.ToString("yyyy-MM-dd HH:mm"),
            isRead = false,
            relatedClientId = client.Id,
            clientName = client.Name + " " + client.Surname
        }
    }
            });
        }


    }


}

