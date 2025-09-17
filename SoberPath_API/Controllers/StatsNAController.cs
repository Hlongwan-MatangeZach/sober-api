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
            try
            {
                // Use Address instead of Location
                var locationData = await _context.Clients
                    .Where(cl => !string.IsNullOrEmpty(cl.Location))
                    .GroupBy(cl => cl.Location)
                    .Select(g => new
                    {
                        address = g.Key,  // Changed from location to address
                        clients = g.Count()
                    })
                    .Where(x => x.clients > 0)
                    .ToListAsync();

                return Ok(locationData);
            }
            catch (Exception ex)
            {
                // Log the error and return empty array
                Console.WriteLine($"Error in LocationvsClients_GraphData: {ex.Message}");
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

        // ---- Substances by gender ----
        [HttpGet("SubstanceByGenderForSocialWorkers")]
        public async Task<ActionResult> GetSubstanceStatsByGenderForSocialWorkers()
        {
            try
            {
                var substanceStats = await (from client in _context.Clients
                                            join substance in _context.Substances on client.Id equals substance.ClientId
                                            where client.Gender != null && substance.Name != null
                                            group new { client, substance } by new
                                            {
                                                Substance = substance.Name.Trim().ToLower(),
                                                Gender = client.Gender.Trim().ToLower()
                                            } into g
                                            select new
                                            {
                                                substance = g.Key.Substance,
                                                gender = g.Key.Gender,
                                                count = g.Count()
                                            })
                                          .OrderByDescending(x => x.count)
                                          .ThenBy(x => x.substance)
                                          .ToListAsync();

                return Ok(substanceStats);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubstanceByGenderForSocialWorkers: {ex.Message}");
                return Ok(new List<object>());
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
