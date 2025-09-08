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
