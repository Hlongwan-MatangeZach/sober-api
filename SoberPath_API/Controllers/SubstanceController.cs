using Microsoft.AspNetCore.Mvc;
using SoberPath_API.Context;

using SoberPath_API.Context;

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
    public class SubstanceController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

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

    }
}
