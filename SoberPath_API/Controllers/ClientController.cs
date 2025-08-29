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
                .Include(c => c.SessionBooking)
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
            _context.SessionBookings.RemoveRange(client.SessionBooking);
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

        [HttpPost("UpdateApplication/{clientId}/{newStatus}/{comment}")]
        public async Task<ActionResult> UpdateApplicationStatus(int clientId, string newStatus, string comment)
        {

            var application = await _context.Applications.Where(app => app.ClientId == clientId).FirstOrDefaultAsync();
            if (application == null)
            {
                return NotFound();
            }


            application.Status = newStatus;
            application.Update_Comment = comment;
            application.Status_Update_Date = DateTime.Now.Date.ToString();
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

        [HttpPost("ADD_SUBSTANCES")]
        public async Task<ActionResult<Substance>> Add_Substances(Substance[] substances)
        {
            if (substances == null)
            {
                return BadRequest();
            }

            foreach (Substance s in substances)
            {
                _context.Substances.Add(s);

            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Get_Client_Substances/{Id}")]

        public async Task<ActionResult<Substance>> Get_Client_Substance(int Id)
        {
            var substances = await _context.Substances.Where(s => s.ClientId == Id).Select(s => new { s.Id, s.Name, s.Description }).ToListAsync();
            if (substances == null)
            {
                return NotFound();
            }



            return Ok(substances);
        }

        [HttpPost("ADD_NOKS")]
        public async Task<ActionResult<Next_of_Kin>> Add_NOKS_List(Next_of_Kin[] noks)
        {
            if (noks == null)
            {
                return BadRequest();
            }

            foreach (Next_of_Kin nok in noks)
            {
                _context.Next_Of_Kins.Add(nok);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Edit_Substance/{clientid}/{substanceid}")]
        public async Task<ActionResult<Substance>> EditSubstance(int clientid, int substanceid, Substance newSubstance)
        {
            var found_substance = await _context.Substances.Where(s => s.Id == substanceid && s.ClientId == clientid).FirstOrDefaultAsync();
            if (substanceid > 0 && clientid > 0 && newSubstance != null)
            {

                if (found_substance != null)
                {
                    if (newSubstance.Name != null)
                    {
                        found_substance.Name = newSubstance.Name;
                    }
                    if (newSubstance.Description != null)
                    {
                        found_substance.Description = newSubstance.Description;
                    }
                    if (newSubstance.ClientId > 0)
                    {
                        found_substance.ClientId = newSubstance.ClientId;
                    }
                }
                else
                {
                    return BadRequest("Substance not found");
                }
            }
            else
            {
                return BadRequest("ID and Substance objects are null");
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("Delete_Substance/{id}")]
        public async Task<IActionResult> DeleteSubstance(int id)
        {
            var substance = await _context.Substances.FindAsync(id);
            if (substance == null)
                return NotFound();

            // Delete related records first
            var relatedRecords = _context.Records.Where(r => r.SubstanceId == id);
            _context.Records.RemoveRange(relatedRecords);

            _context.Substances.Remove(substance);
            await _context.SaveChangesAsync();

            return Ok("Substance deleted successfully");
        }


        [HttpGet("Next_of_Kin_list/{Id}")]
        public async Task<ActionResult<IEnumerable<Next_of_Kin>>> GetNextOfKin(int Id)
        {
            var nextOfKins = await _context.Next_Of_Kins
                .Where(nk => nk.ClientId == Id)
                .ToListAsync();
            if (nextOfKins == null || !nextOfKins.Any())
            {
                return NotFound();
            }
            return Ok(nextOfKins);
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

        [HttpPost("Edit_Next_of_Kin/{cliendid}")]
        public async Task<ActionResult<Next_of_Kin>> EditNextOfKin(int cliendid, Next_of_Kin newNextOfKin)
        {
            var found_next_of_kin = await _context.Next_Of_Kins.Where(nk => nk.ClientId == cliendid).FirstOrDefaultAsync();
            if (cliendid > 0 && newNextOfKin != null)
            {

                if (found_next_of_kin != null)
                {
                    if (newNextOfKin.Name != null)
                    {
                        found_next_of_kin.Name = newNextOfKin.Name;
                    }
                    if (newNextOfKin.Relationship != null)
                    {
                        found_next_of_kin.Relationship = newNextOfKin.Relationship;
                    }
                    if (newNextOfKin.Phone_number != null)
                    {
                        found_next_of_kin.Phone_number = newNextOfKin.Phone_number;
                    }
                }
                else if (found_next_of_kin == null)
                {
                    if (newNextOfKin != null)
                    {
                        _context.Next_Of_Kins.Add(newNextOfKin);

                    }

                    await _context.SaveChangesAsync();
                    return BadRequest("ID and Next of Kin objects are null");

                }
            }
            else
            {
                return BadRequest("Null input details");

            }
            await _context.SaveChangesAsync();
            return NoContent();
        }



        [HttpPost("AddRecords")]
        public async Task<IActionResult> AddRecording([FromBody] Records recording)
        {
            if (recording == null)
                return BadRequest("Invalid data");

            recording.RecordedDate = recording.RecordedDate;
            _context.Records.Add(recording);
            await _context.SaveChangesAsync();
            return Ok(recording);
        }


        [HttpDelete("DeleteRecord/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var found = await _context.Records.FindAsync(id);
            if (found == null)
                return NotFound();

            _context.Records.Remove(found);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("UpdateRecord/{id}")]
        public async Task<IActionResult> UpdateRecording(int id, [FromBody] Records updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Records.FindAsync(id);
            if (existing == null)
                return NotFound("Recording not found");

            // Optional safety check
            if (updated.Id != id)
                return BadRequest("ID mismatch between URL and body");

            existing.Quantity = updated.Quantity;
            existing.SubstanceId = updated.SubstanceId;
            existing.RecordedDate = updated.RecordedDate;
            existing.ClientId = updated.ClientId;

            await _context.SaveChangesAsync();

            return Ok(existing);
        }

        [HttpGet("GetRecordByDate/{clientId}/{date}")]
        public async Task<IActionResult> GetRecordByDate(int clientId, string date)
        {
            var records = await _context.Records
                .Where(r => r.ClientId == clientId && r.RecordedDate == DateOnly.Parse(date))
                .Select(r => new {
                    r.SubstanceId,
                    r.Quantity
                })
                .ToListAsync();

            return Ok(records);
        }

        [HttpGet("GetSubstanceTrends/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecordById(int id)
        {
            var substanceReports = await _context.Records
                .Where(r => r.ClientId == id && r.SubstanceId.HasValue)
                .Join(
                    _context.Substances,
                    record => record.SubstanceId,
                    substance => substance.Id,
                    (record, substance) => new { record, substance }
                )
                .GroupBy(joined => new { joined.substance.Id, joined.substance.Name })
                .Select(group => new
                {
                    substanceName = group.Key.Name,
                    records = group.Select(x => new
                    {
                        date = x.record.RecordedDate,
                        quantity = x.record.Quantity
                    })
                    .OrderBy(x => x.date)
                    .ToList()
                })
                .ToListAsync();

            return Ok(substanceReports);
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
                //id=application.Id,
                applicationDate = application.Date,
                reasonForRehab = application.Reason,
                editableReason = application.RehabReason,
                summary = application.Summary,
                addictionLevel = application.Addiction_level,
                substances = _context.Substances.Where(sub => sub.ClientId == id).Select(sub => sub.Name).ToList(),
                socialWorkerName = _context.Social_Workers.Where(sw => sw.Id == application.Social_WorkerId).Select(sw => sw.Name).FirstOrDefault(),

            };

            return Ok(returnval);
        }
        [HttpPost("EditApplicationReason/{clientid}/{value}")]

        public async Task<ActionResult> EditApplicationStatus(int clientid, string value)
        {
            var application = await _context.Applications.Where(app => app.ClientId == clientid).FirstOrDefaultAsync();
            if (application == null)
            {
                return NotFound();
            }

            application.RehabReason = value;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("GetAddiction/{clientId}")]
        public async Task<ActionResult<ProgressDto>> GetProgress(int clientId)
        {
            // Get substances with their records
            var substances = await _context.Substances
                .Include(s => s.Records)
                .Where(s => s.ClientId == clientId)
                .ToListAsync();

            if (!substances.Any())
            {
                return NotFound("No substances found for this client");
            }

            // Calculate progress (defaults to current month)
            var result = CalculateProgress(clientId, substances);
            return Ok(result);
        }

     
        private ProgressDto CalculateProgress(int clientId, List<Substance> substances)
        {
            // Always default to current month
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var startDateOnly = DateOnly.FromDateTime(startDate);
            var endDateOnly = DateOnly.FromDateTime(endDate);

            // Calculate total days once (was missing this variable)
            var totalDays = (endDate - startDate).Days + 1;

            var result = new ProgressDto
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                TotalDays = totalDays,
                SubstanceProgressDetails = new List<SubstanceProgress>() // Initialize the list
            };

            var successfulDays = new HashSet<DateOnly>();
            var allDays = Enumerable.Range(0, totalDays)
                .Select(offset => startDateOnly.AddDays(offset))
                .ToList();

            foreach (var substance in substances.Where(s => s.Records != null))
            {
                var substanceProgress = new SubstanceProgress
                {
                    SubstanceId = substance.Id,
                    SubstanceName = substance.Name,
                    DaysWithinThreshold = 0,  // Explicit initialization
                    DaysExceededThreshold = 0
                };

                var dailyUsage = substance.Records
                    .Where(r => r.RecordedDate >= startDateOnly &&
                               r.RecordedDate <= endDateOnly)
                    .GroupBy(r => r.RecordedDate)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(r => r.Quantity ?? 0)  // Handle null quantities
                    );

                foreach (var day in allDays)
                {
                    if (dailyUsage.TryGetValue(day, out var quantity))
                    {
                        if (quantity <= (substance.DailyThreshold ?? 0))
                        {
                            successfulDays.Add(day);
                            substanceProgress.DaysWithinThreshold++;
                        }
                        else
                        {
                            substanceProgress.DaysExceededThreshold++;
                        }
                    }
                    else
                    {
                        successfulDays.Add(day);
                        substanceProgress.DaysWithinThreshold++;
                    }
                }

                result.SubstanceProgressDetails.Add(substanceProgress);
            }

            result.CompletedDays = successfulDays.Count;
            result.ProgressPercentage = result.TotalDays > 0
                ? (double)result.CompletedDays / result.TotalDays * 100
                : 0;  // Prevent division by zero

            return result;
        }
    }


}

