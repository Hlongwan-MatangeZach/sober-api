using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // ---- SW vs Clients ----
        [HttpGet("SWvsClients_GraphData")]
        public async Task<ActionResult> Get_SWvsClients()
        {
            var result = await _context.Social_Workers
                .Where(sw => sw.Name != null)
                .Select(sw => new
                {
                    social_worker = sw.Name,
                    No_of_clients = _context.Clients.Count(cl => cl.Social_WorkerId != null && cl.Social_WorkerId == sw.Id)
                })
                .Where(x => x.No_of_clients > 0) // Only return social workers with clients
                .ToListAsync();

            return Ok(result);
        }

        // Backwards-compatible alias
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
        [HttpGet("Ave_No_Clients_per_SW")]
        public async Task<ActionResult<double>> GetAverageNumber()
        {
            var socialWorkersWithClients = await _context.Social_Workers
                .Select(sw => new
                {
                    ClientCount = _context.Clients.Count(cl => cl.Social_WorkerId == sw.Id)
                })
                .Where(x => x.ClientCount > 0)
                .ToListAsync();

            if (!socialWorkersWithClients.Any())
                return Ok(0.0);

            var avg = socialWorkersWithClients.Average(x => x.ClientCount);
            return Ok(Math.Round(avg, 2));
        }

        // Backwards-compatible alias
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
            var genderStats = await _context.Clients
                .Where(cl => cl.Gender != null)
                .GroupBy(cl => cl.Gender.ToLower())
                .Select(g => new
                {
                    id = g.Key,
                    label = g.Key,
                    value = g.Count(),
                    color = g.Key == "male" ? "hsl(104, 70%, 50%)" :
                            g.Key == "female" ? "hsl(162, 70%, 50%)" :
                            "hsl(280, 70%, 50%)"
                })
                .ToListAsync();

            return Ok(genderStats);
        }

        // ---- Most used substances ----
        public class SubstanceCountDto
        {
            public string Name { get; set; } = string.Empty;
            public int Count { get; set; }
        }

        [HttpGet("MostUsedSubstances")]
        public async Task<ActionResult<IEnumerable<SubstanceCountDto>>> GetMostUsedSubstances(int top = 10)
        {
            top = Math.Max(1, Math.Min(100, top));

            var substances = await _context.Substances
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .GroupBy(s => s.Name.Trim().ToLower())
                .Select(g => new SubstanceCountDto
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .Take(top)
                .ToListAsync();

            return Ok(substances);
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

        // ---- Clients status trend ----
        [HttpGet("Clients_Status_Trend_LineGraph")]
        public async Task<ActionResult> Get_ClientsStatusTrend_Data()
        {
            try
            {
                var applications = await _context.Applications
                    .Where(app => app.Status != null && app.Date != null && app.Date.Length >= 7)
                    .Select(app => new
                    {
                        app.Status,
                        Year = app.Date.Substring(0, 4),
                        Month = app.Date.Substring(5, 2)
                    })
                    .ToListAsync();

                var statuses = new[] { "Pending", "Approved", "Rejected", "Discharged" };

                var result = statuses.Select(status => new
                {
                    id = status,
                    data = applications
                        .Where(app => app.Status == status)
                        .GroupBy(app => new { app.Year, app.Month })
                        .Select(g => new
                        {
                            x = $"{GetMonthName(g.Key.Month)} {g.Key.Year}",
                            y = g.Count()
                        })
                        .OrderBy(d => d.x) // Sort by date string
                        .ToList()
                })
                .Where(x => x.data.Any()) // Only include statuses with data
                .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Clients_Status_Trend_LineGraph: {ex.Message}");
                return Ok(new List<object>());
            }
        }

        private static string GetMonthName(string monthNumber)
        {
            return monthNumber switch
            {
                "01" => "Jan",
                "02" => "Feb",
                "03" => "Mar",
                "04" => "Apr",
                "05" => "May",
                "06" => "Jun",
                "07" => "Jul",
                "08" => "Aug",
                "09" => "Sep",
                "10" => "Oct",
                "11" => "Nov",
                "12" => "Dec",
                _ => monthNumber
            };
        }

        
    }
}