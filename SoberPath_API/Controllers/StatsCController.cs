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

        [HttpGet("GetWeeklyStats/{clientId}")]
        public async Task<ActionResult<IEnumerable<WeeklyStatsDto>>> GetWeeklyStats(int clientId)
        {
            var substances = await _context.Substances
                .Include(s => s.Records)
                .Where(s => s.ClientId == clientId)
                .ToListAsync();

            if (!substances.Any())
                return NotFound("No substances found for this client.");

            var results = new List<WeeklyStatsDto>();

            foreach (var substance in substances)
            {
                var records = substance.Records ?? new List<Records>();

                // Group records by ISO week number
                var weeklyData = records
                .Where(r => r.RecordedDate != null && r.Quantity != null)
                .GroupBy(r => GetIso8601WeekOfYear(r.RecordedDate.Value.ToDateTime(TimeOnly.MinValue)))
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    WeekNumber = "W" + g.Key,
                    Total = g.Sum(r => r.Quantity ?? 0)
                })
                .ToList();


                var dto = new WeeklyStatsDto
                {
                    SubstanceName = substance.Name ?? "Unknown",
                    Unit = substance.unit ?? "",
                    WeekNumbers = weeklyData.Select(w => w.WeekNumber).ToList(),
                    WeeklyTotals = weeklyData.Select(w => w.Total).ToList()
                };

                results.Add(dto);
            }

            return Ok(results);
        }
        private static int GetIso8601WeekOfYear(DateTime date)
        {
            // ISO 8601 week starts on Monday
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                date = date.AddDays(3); // move date to Thursday of the same week
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday
            );
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

            // Calculate progress + addiction rate
            var result = CalculateProgress(clientId, substances);
            return Ok(result);
        }

        private ProgressDto CalculateProgress(int clientId, List<Substance> substances)
        {
            var result = new ProgressDto
            {
                ClientId = clientId,
                TimeframeDescription = "Lifetime",
                SubstanceProgressDetails = new List<SubstanceProgress>()
            };

            // Get all unique days where at least one substance has a record
            var allRecordedDays = substances
                .Where(s => s.Records != null)
                .SelectMany(s => s.Records)
                .Where(r => r.RecordedDate.HasValue)
                .Select(r => r.RecordedDate.Value)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (!allRecordedDays.Any())
            {
                result.TotalDays = 0;
                result.CompletedDays = 0;
                result.ProgressPercentage = 0;
                result.AddictionRate = 0;

                foreach (var substance in substances)
                {
                    result.SubstanceProgressDetails.Add(new SubstanceProgress
                    {
                        SubstanceId = substance.Id,
                        SubstanceName = substance.Name,
                        DaysWithinThreshold = 0,
                        DaysExceededThreshold = 0
                    });
                }

                return result;
            }

            result.TotalDays = allRecordedDays.Count;
            double addictionRate = 0.0;

            // Calculate progress for each substance
            foreach (var substance in substances.Where(s => s.Records != null))
            {
                var substanceProgress = new SubstanceProgress
                {
                    SubstanceId = substance.Id,
                    SubstanceName = substance.Name,
                    DaysWithinThreshold = 0,
                    DaysExceededThreshold = 0
                };

                // Group usage by date for this substance
                var dailyUsage = substance.Records
                    .Where(r => r.RecordedDate.HasValue)
                    .GroupBy(r => r.RecordedDate.Value)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(r => r.Quantity ?? 0)
                    );

                // Check each day that has at least one substance record
                foreach (var day in allRecordedDays)
                {
                    if (dailyUsage.TryGetValue(day, out var quantity))
                    {
                        if (quantity <= (substance.DailyThreshold ?? 0))
                        {
                            substanceProgress.DaysWithinThreshold++;
                        }
                        else
                        {
                            substanceProgress.DaysExceededThreshold++;
                            addictionRate += 0.2; // Rule 2: exceeded threshold
                        }
                    }
                    else
                    {
                        // No usage recorded for this substance on this day - count as within threshold
                        substanceProgress.DaysWithinThreshold++;
                    }
                }

                result.SubstanceProgressDetails.Add(substanceProgress);
            }

            // Calculate overall success (Days where ALL substances were within threshold)
            var successfulDays = 0;

            foreach (var day in allRecordedDays)
            {
                bool dayIsSuccessful = true;

                foreach (var substance in substances.Where(s => s.Records != null))
                {
                    var substanceRecord = substance.Records
                        .FirstOrDefault(r => r.RecordedDate == day);

                    if (substanceRecord != null &&
                        (substanceRecord.Quantity ?? 0) > (substance.DailyThreshold ?? 0))
                    {
                        dayIsSuccessful = false;
                        break;
                    }
                }

                if (dayIsSuccessful)
                {
                    successfulDays++;
                }
            }

            result.CompletedDays = successfulDays;
            result.ProgressPercentage = result.TotalDays > 0
                ? (double)result.CompletedDays / result.TotalDays * 100
                : 0;

            // Addiction rate rule 1: 2 days in a row consumption
            for (int i = 1; i < allRecordedDays.Count; i++)
            {
                var prevDay = allRecordedDays[i - 1];
                var currDay = allRecordedDays[i];

                if ((currDay.DayNumber - prevDay.DayNumber) == 1) // DateOnly consecutive check
                {
                    addictionRate += 0.4;
                }
            }

            // Addiction rate rule 3: no usage for a full week
            for (int i = 1; i < allRecordedDays.Count; i++)
            {
                var prevDay = allRecordedDays[i - 1];
                var currDay = allRecordedDays[i];

                if ((currDay.DayNumber - prevDay.DayNumber) >= 7)
                {
                    addictionRate -= 0.2;
                }
            }

            result.AddictionRate = Math.Max(0, addictionRate); // prevent negative

            return result;
        }

        [HttpGet("GetSoberStreak/{clientId}/{substanceId}")]
        public async Task<ActionResult<int>> GetSoberStreak(
            int clientId,
            [FromQuery] int? substanceId = null)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            // Filter by client (and substance if provided)
            var query = _context.Records.AsQueryable()
                .Where(r => r.ClientId == clientId);

            if (substanceId.HasValue)
            {
                query = query.Where(r => r.SubstanceId == substanceId.Value);
            }

            // Group by date and sum quantities
            var records = await query
                .GroupBy(r => r.RecordedDate)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity ?? 0)
                })
                .ToListAsync();

            var recordDict = records
                .Where(r => r.Date != null)
                .ToDictionary(r => r.Date!.Value, r => r.TotalQuantity);

            int streak = 0;
            var checkDate = today;

            while (true)
            {
                if (recordDict.TryGetValue(checkDate, out var qty))
                {
                    if (qty > 0)
                        break; // substance used -> streak ends
                }
                else
                {
                    // No record = sober day
                }

                streak++;
                checkDate = checkDate.AddDays(-1);
            }

            return Ok(streak);
        }

    }
}
