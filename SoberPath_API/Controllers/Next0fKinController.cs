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
    public class NextOfKinController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

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


    }
}
