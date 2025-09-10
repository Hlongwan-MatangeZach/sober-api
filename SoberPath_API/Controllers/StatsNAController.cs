using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SoberPath_API.Controllers.StatsController;

namespace SoberPath_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsNAController : ControllerBase
    {
        private readonly Sober_Context _context;

        public StatsNAController(Sober_Context context)
        {
            _context = context;
        }

        // ---- SW vs Clients (canonical) ----
        [HttpGet("SWvsClients_GraphData")]
        public async Task<ActionResult> Get_SWvsClients()
        {
            var retuurnal = await _context.Social_Workers
                .Select(sw => new
                {
                    social_worker = sw.Name,
                    No_of_clients = _context.Clients.Count(cl => cl.Social_WorkerId != null && cl.Social_WorkerId == sw.Id)
                })
                .ToListAsync();

            return Ok(retuurnal);
        }

        // Backwards-compatible alias for the misspelled route some clients may call
        [HttpGet("SWvsClienst_GraphData")]
        public Task<ActionResult> Get_SWvsClients_OldAlias() => Get_SWvsClients();

        // ---- Location vs Clients ----
        [HttpGet("LocationvsClients_GraphData")]
        public async Task<ActionResult> Get_LocationvsClients()
        {
            // Defensive: if the Clients table doesn't have Location data, return an empty list
            // If Location column exists and contains values, this will group and return counts.
            // Uses server-side grouping for efficiency.
            try
            {
                var hasLocationColumn = _context.Model.FindEntityType(typeof(Client))?
                    .GetProperties().Any(p => string.Equals(p.Name, "Location", StringComparison.OrdinalIgnoreCase)) ?? false;

                if (!hasLocationColumn)
                    return Ok(new List<object>());

                var query = await _context.Clients
                    .Where(cl => !string.IsNullOrEmpty(cl.Address))
                    .GroupBy(cl => cl.Address)
                    .Select(g => new
                    {
                        location_ = g.Key,
                        Number = g.Count()
                    })
                    .ToListAsync();

                return Ok(query);
            }
            catch (Exception)
            {
                // If anything goes wrong (e.g., column missing or migration mismatch), return empty list so frontend can handle it
                return Ok(new List<object>());
            }
        }

        // ---- Average number of clients per social worker ----
        // Canonical route (safe name)
        [HttpGet("Ave_No_Clients_per_SW")]
        public async Task<ActionResult<double>> GetAverageNumber()
        {
            // Get counts per SW then average (execute in-memory to avoid SQL translation edge cases)
            var counts = await _context.Social_Workers
                .Select(sww => _context.Clients.Count(cl => cl.Social_WorkerId == sww.Id))
                .ToListAsync();

            if (!counts.Any())
                return Ok(0.0);

            var avg = counts.Average();
            return Ok(Math.Round(avg, 2));
        }

        // Backwards-compatible alias for old route name with punctuation / underscores
        [HttpGet("Ave_No._Clients__per_SW")]
        public Task<ActionResult<double>> GetAverageNumber_OldAlias() => GetAverageNumber();

        // ---- Totals ----
        [HttpGet("Totals")]
        public async Task<ActionResult> GetTotals()
        {
            var totalClients = await _context.Clients.CountAsync();
            var totalSocialWorkers = await _context.Social_Workers.CountAsync();
            var totalNGOAdmins = await _context.NGO_Admins.CountAsync();
            var totalRehabAdmins = await _context.Rehab_Admins.CountAsync();

            return Ok(new
            {
                totalClients,
                totalSocialWorkers,
                totalNGOAdmins,
                totalRehabAdmins
            });
        }

        // ---- Clients by gender ----
        [HttpGet("Clients_Gender_Stats")]
        public async Task<ActionResult> Get_Client_Stats_byGender()
        {
            var totalClients = await _context.Clients.CountAsync();
            var sum_males = await _context.Clients.CountAsync(cl => cl.Gender != null && cl.Gender.ToLower() == "male");
            var sum_females = await _context.Clients.CountAsync(cl => cl.Gender != null && cl.Gender.ToLower() == "female");
            var sum_other = await _context.Clients.CountAsync(cl => cl.Gender != null && cl.Gender.ToLower() == "other");

            double female_percentage = totalClients == 0 ? 0 : Math.Round(((double)sum_females / totalClients) * 100);
            double male_percentage = totalClients == 0 ? 0 : Math.Round(((double)sum_males / totalClients) * 100);
            double other_percentage = totalClients == 0 ? 0 : Math.Round(((double)sum_other / totalClients) * 100);

            var returnval = await _context.Clients
                .Where(cl => cl.Gender != null)
                .GroupBy(cl => cl.Gender)
                .Where(g => g.Key != null)
                .Select(g => new
                {
                    id = g.Key!.ToLower(),
                    label = g.Key.ToLower(),
                    value = g.Key.ToLower() == "male" ? male_percentage : g.Key.ToLower() == "female" ? female_percentage : other_percentage,
                    color = g.Key.ToLower() == "male" ? "hsl(104, 70%, 50%)" : g.Key.ToLower() == "female" ? "hsl(162, 70%, 50%)" : "hsla(162, 62%, 29%, 1.00)"
                })
                .ToListAsync();

            return Ok(returnval);
        }

        // ---- Most used substances (top N) ----
        [HttpGet("MostUsedSubstances")]
        public async Task<ActionResult<IEnumerable<SubstanceCountDto>>> GetMostUsedSubstances(int top = 1, bool includeTies = false)
        {
            top = Math.Max(1, Math.Min(100, top));

            var groupedQuery = _context.Substances
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .GroupBy(s => s.Name!.Trim().ToLower())
                .Select(g => new SubstanceCountDto
                {
                    Name = g.Key!,
                    Count = g.Count()
                });

            var topList = await groupedQuery
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .Take(top)
                .AsNoTracking()
                .ToListAsync();

            if (!includeTies) return Ok(topList);

            if (!topList.Any()) return Ok(topList);

            var maxCount = topList.Max(t => t.Count);

            var tied = await groupedQuery
                .Where(g => g.Count == maxCount)
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .AsNoTracking()
                .ToListAsync();

            return Ok(tied);
        }

        // ---- Substances broken down by client gender for social workers ----
        [HttpGet("SubstanceByGenderForSocialWorkers")]
        public async Task<ActionResult> GetSubstanceStatsByGenderForSocialWorkers()
        {
            try
            {
                var substanceStats = await _context.Social_Workers
                    .Join(_context.Clients,
                          sw => sw.Id,
                          c => c.Social_WorkerId,
                          (sw, client) => new { sw, client })
                    .Join(_context.Substances,
                          joined => joined.client.Id,
                          s => s.ClientId,
                          (joined, substance) => new { joined.sw, joined.client, substance })
                    .Where(joined => joined.client.Gender != null && joined.substance.Name != null)
                    .GroupBy(joined => new { SubstanceName = joined.substance.Name, joined.client.Gender })
                    .Select(g => new
                    {
                        substance = g.Key.SubstanceName!.ToLower(),
                        gender = g.Key.Gender!.ToLower(),
                        count = g.Count()
                    })
                    .OrderByDescending(result => result.count)
                    .ToListAsync();

                if (!substanceStats.Any()) return Ok(new List<object>());
                return Ok(substanceStats);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // ---- Clients status trend (line graph data) ----
        private static string GetMonthName(string str)
        {
            return str switch
            {
                "01" => "January",
                "02" => "February",
                "03" => "March",
                "04" => "April",
                "05" => "May",
                "06" => "June",
                "07" => "July",
                "08" => "August",
                "09" => "September",
                "10" => "October",
                "11" => "November",
                "12" => "December",
                _ => ""
            };
        }

        [HttpGet("Clients_Status_Trend_LineGraph")]
        public async Task<ActionResult> Get_ClientsStatusTrend_Data()
        {
            var rawData = await _context.Applications
                .Where(app => app.Status != null && app.Date != null &&
                             (app.Status == "Pending" || app.Status == "Approved" ||
                              app.Status == "Rejected" || app.Status == "Discharged"))
                .Select(app => new
                {
                    Status = app.Status!,
                    MonthNumber = app.Date!.Substring(5, 2),
                    Year = app.Date.Substring(0, 4)
                })
                .ToListAsync();

            var returnval = rawData
                .GroupBy(app => app.Status)
                .Select(group => new
                {
                    id = group.Key,
                    data = group
                        .GroupBy(x => new { x.MonthNumber, x.Year })
                        .Select(g => new
                        {
                            MonthNumber = g.Key.MonthNumber,
                            Year = g.Key.Year,
                            MonthName = GetMonthName(g.Key.MonthNumber),
                            Count = g.Count(),
                            SortableDate = Convert.ToInt32(g.Key.Year + g.Key.MonthNumber)
                        })
                        .OrderBy(d => d.SortableDate)
                        .Select(d => new
                        {
                            x = $"{d.MonthName} {d.Year}",
                            y = d.Count
                        })
                        .ToList()
                })
                .ToList();

            return Ok(returnval);
        }
    }
}
