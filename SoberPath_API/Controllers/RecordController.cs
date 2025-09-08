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
    public class RecordController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

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



    }
}
