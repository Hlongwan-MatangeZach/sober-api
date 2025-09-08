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
    public class StatsCController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpGet("GetThresholdStats/{clientId}")]
        public async Task<ActionResult<IEnumerable<ThresholdStatsDto>>> GetThresholdStats(int clientId)
        {
            var substances = await _context.Substances
                .Include(s => s.Records)
                .Where(s => s.ClientId == clientId)
                .ToListAsync();

            if (!substances.Any())
                return NotFound("No substances found for this client.");

            var results = new List<ThresholdStatsDto>();

            foreach (var substance in substances)
            {
                var records = substance.Records ?? new List<Records>();

                var totalDays = records.Count;
                var daysExceeding = records.Count(r => r.Quantity.HasValue && substance.DailyThreshold.HasValue && r.Quantity.Value > substance.DailyThreshold.Value);
                var maxOverThreshold = records
                    .Where(r => r.Quantity.HasValue && substance.DailyThreshold.HasValue && r.Quantity.Value > substance.DailyThreshold.Value)
                    .Select(r => r.Quantity.Value - substance.DailyThreshold.Value)
                    .DefaultIfEmpty(0)
                    .Max();

                var safePercentage = totalDays > 0 ? ((double)(totalDays - daysExceeding) / totalDays) * 100 : 0;

                results.Add(new ThresholdStatsDto
                {
                    SubstanceName = substance.Name ?? "Unknown",
                    Unit = substance.unit ?? "",
                    DaysExceedingThreshold = daysExceeding,
                    TotalDays = totalDays,
                    MaxOverThreshold = maxOverThreshold,
                    PercentageSafeDays = Math.Round(safePercentage, 2)
                });
            }

            return Ok(results);
        }


        
    }
}
