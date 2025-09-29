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
    public class StatsRAController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

       [HttpGet("Total_clients_inhouse")]
        public async Task<ActionResult> GetTotalClients_inhouse()
        {
            var num = await _context.Applications.Where(app => app.Status != null && app.Status == "Approved & Allocated" && app.ClientId != null).CountAsync();

            if (num <= 0)
            {
                return BadRequest();
            }

            return Ok(num);
        }

        [HttpGet("Total_clients_history")]
        public async Task<ActionResult> GetTotalClients()
        {
            var num = await _context.Applications.Where(app => app.Status != null && app.Status == "Approved" || app.Status == "Approved & Allocated" || app.Status == "Discharged" && app.ClientId != null).CountAsync();

            if (num <= 0)
            {
                return BadRequest();
            }

            return Ok(num);
        }

        [HttpGet("Approved_Gender_Stats")]

        public async Task<IActionResult> Get_GenderPercentage()
        {
            var admitted_clients_list = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && app.Status == "Approved" || app.Status == "Approved & Allocated").CountAsync();
            var male_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.Id && cl.Gender != null && cl.Gender.ToLower() == "male")).CountAsync();
            var female_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.Id && cl.Gender != null && cl.Gender.ToLower() == "female")).CountAsync();
            var female_list_count_ = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.Id && cl.Gender != null && cl.Gender.ToLower() == "female")).Select(app => new
            {
                clientId = app.ClientId,
                gender = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Gender).FirstOrDefault()
            }).ToListAsync();
            var other_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && app.Status == "Approved" || app.Status == "Approved & Allocated" && _context.Clients.Any(cl => cl.Id == app.Id && cl.Gender == "other")).CountAsync();




            if (admitted_clients_list == 0)
            {
                return BadRequest("Zero clients admitted");
            }


            var percent_female_client = Math.Round(((decimal)female_list_count / admitted_clients_list * 100), 2);
            var percent_male_client = Math.Round(((decimal)male_list_count / admitted_clients_list * 100), 2);
            var percent_other_client = Math.Round(((decimal)other_list_count / admitted_clients_list * 100), 2);


            var returnval = await _context.Clients.Where(cl => cl.Gender != null).GroupBy(cl => cl.Gender).Where(g => g.Key != null).Select(g => new
            {
                id = g.Key!.ToLower(),
                label = g.Key.ToLower(),
                value = g.Key.ToLower() == "male" ? percent_male_client : g.Key.ToLower() == "female" ? percent_female_client : percent_other_client,
                color = g.Key.ToLower() == "male" ? "hsl(104, 70%, 50%)" : g.Key.ToLower() == "female" ? "hsl(162, 70%, 50%)" : "hsla(162, 62%, 29%, 1.00)",

            }).ToListAsync();


            return Ok(returnval);



        }

        [HttpGet("Approved_Race_Stats")]

        public async Task<IActionResult> Get_Client_RaceStats()
        {
            var admitted_clients_list = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated")).CountAsync();
            var white_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.ClientId && cl.Race != null && cl.Race.ToLower() == "white")).CountAsync();
            var black_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.ClientId && cl.Race != null && cl.Race.ToLower() == "black")).CountAsync();
            var black_list_count_ = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.ClientId && cl.Race != null && cl.Race.ToLower() == "black")).Select(app => new { clientId = app.ClientId }).ToListAsync();
            var coloured_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.ClientId && cl.Race != null && cl.Race.ToLower() == "coloured")).CountAsync();

            var indian_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.ClientId && cl.Race != null && cl.Race.ToLower() == "indian")).CountAsync();

            var other_list_count = await _context.Applications.Where(app => app.ClientId != null && app.Social_WorkerId != null && (app.Status == "Approved" || app.Status == "Approved & Allocated") && _context.Clients.Any(cl => cl.Id == app.ClientId && cl.Race != null && cl.Race.ToLower() == "other")).CountAsync();


            if (admitted_clients_list == 0)
            {
                return BadRequest("Zero clients admitted");
            }


            var percent_black_client = Math.Round(((decimal)black_list_count / admitted_clients_list * 100), 2);
            var percent_white_client = Math.Round(((decimal)white_list_count / admitted_clients_list * 100), 2);
            var percent_coloured_client = Math.Round(((decimal)coloured_list_count / admitted_clients_list * 100), 2);
            var percent_indian_client = Math.Round(((decimal)indian_list_count / admitted_clients_list * 100), 2);
            var percent_other_client = Math.Round(((decimal)other_list_count / admitted_clients_list * 100), 2);


            var returnval = await _context.Clients.Where(cl => cl.Race != null).GroupBy(cl => cl.Race).Where(g => g.Key != null).Select(g => new
            {
                id = g.Key!.ToLower(),
                label = g.Key.ToLower(),
                value = g.Key.ToLower() == "black" ? percent_black_client : g.Key.ToLower() == "white" ? percent_white_client : g.Key.ToLower() == "coloured" ? percent_coloured_client : g.Key.ToLower() == "indian" ? percent_indian_client : percent_other_client,
                color = g.Key.ToLower() == "black" ? "hsl(104, 70%, 50%)" : g.Key.ToLower() == "white" ? "hsl(162, 70%, 50%)" : g.Key.ToLower() == "coloured" ? "hsl(162, 70%, 50%)" : g.Key.ToLower() == "indian" ? "hsl(162, 70%, 50%)" : "hsla(162, 62%, 29%, 1.00)",

            }).ToListAsync();


            return Ok(returnval);
        }

        [HttpGet("Processing_Trend_Data")]
        public async Task<IActionResult> Get_ProcessingTrend()
        {
            var applications = await _context.Applications
                .Where(app => app.Status_Update_Date != null
                        && app.Date != null
                        && (app.Status == "Approved"
                            || app.Status == "Pending"
                            || app.Status == "Rejected"
                            || app.Status == "Approved & Allocated"))
                .Select(app => new
                {
                    Status = app.Status,
                    Creation_date = app.Date,
                    Update_date = app.Status_Update_Date,
                })
                .ToListAsync();

            // Create a list of all months (01 to 12)
            var allMonths = Enumerable.Range(1, 12)
                .Select(i => i.ToString("D2"))
                .ToList();

            // Get the target statuses
            var targetStatuses = new List<string> { "Approved", "Pending", "Rejected", "Approved & Allocated" };

            // Process the data and calculate processing days
            var processedData = applications
                .Select(app => new
                {
                    Status = app.Status,
                    CreationDate = app.Creation_date,
                    UpdateDate = app.Update_date,
                    ProcessingDays = (DateTime.Parse(app.Update_date!) - DateTime.Parse(app.Creation_date!)).TotalDays,
                    MonthNumber = app.Creation_date!.Substring(5, 2)
                })
                .Where(x => x.ProcessingDays >= 0) // Only include valid time spans
                .ToList();

            // Group by status and month
            var groupedData = processedData
                .GroupBy(x => x.Status)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.MonthNumber)
                          .ToDictionary(gg => gg.Key, gg => gg.Average(item => item.ProcessingDays))
                );

            // Build the result with all months included for each status
            var result = targetStatuses
                .Select(status => new
                {
                    id = status,
                    data = allMonths.Select(month => new
                    {
                        x = Get_Month(month),
                        y = groupedData.ContainsKey(status) && groupedData[status].ContainsKey(month)
                            ? Math.Round(groupedData[status][month], 2)
                            : 0
                    }).ToList()
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("Get_current_approvals")]
        public async Task<IActionResult> GetCurrent_approvals()
        {
            var dates = await _context.Applications.Where(app => app.Status != null && app.Status == "Approved & Allocated" || app.Status == "Approved" && app.Date != null).Select(app => new
            {
                date = app.Date,
            }).ToListAsync();

            var count = 0;
            if (dates.Count() <= 0)
            {
                return BadRequest();
            }
            else
            {
                foreach (var app in dates)
                {
                    var int_value = int.Parse(app.date!.Substring(5, 2));
                    if (int_value == DateTime.Now.Month)
                    {
                        count++;
                    }

                }
            }
            return Ok(count);
        }

        [HttpGet("Get_current_month_applications")]

        public async Task<IActionResult> Get_current_month_applications()
        {
            var returnval = await _context.Applications.Where(app => app.ClientId != null && app.Status != null && app.Date != null).Select(app => new
            {
                Date = app.Date,
            }).ToListAsync();


            var result = returnval.Where(app => app.Date != null && DateTime.Parse(app.Date).Month == DateTime.Now.Month).Count();

            return Ok(result);
        }

        [HttpGet("Monthly_Admission_Data")]
        public async Task<IActionResult> GetMonthlyAdmission_Data()
        {
            // Get the relevant data from database
            var applications = await _context.Applications
                .Where(app => (app.Status == "Approved & Allocated" || app.Status == "Discharged")
                        && app.ClientId != null
                        && app.Date != null
                        && app.Status_Update_Date != null)
                .Select(app => new
                {
                    Status = app.Status,
                    DateSubmitted = app.Date,
                    StatusUpdateDate = app.Status_Update_Date,
                    DischargeDate = _context.Rehab_disharges.Where(ds => ds.ApplicationId == app.Id).Select(ds => ds.Disharge_Reason).FirstOrDefault()
                })
                .ToListAsync();

            // Create a list of all months (01 to 12)
            var allMonths = Enumerable.Range(1, 12)
                .Select(i => i.ToString("D2"))
                .ToList();

            // Group admissions and discharges by month
            var admissionsByMonth = applications
                .Where(app => app.Status == "Approved & Allocated" && app.StatusUpdateDate != null)
                .GroupBy(app => app.StatusUpdateDate!.Substring(5, 2))
                .ToDictionary(g => g.Key, g => g.Count());

            var dischargesByMonth = applications
                .Where(app => app.Status == "Discharged" && app.StatusUpdateDate != null)
                .GroupBy(app => app.StatusUpdateDate!.Substring(5, 2))
                .ToDictionary(g => g.Key, g => g.Count());

            // Build the result with all months included
            var result = allMonths
                .Select(month => new
                {
                    Month = Get_Month(month),
                    No_of_Admissions = admissionsByMonth.ContainsKey(month) ? admissionsByMonth[month] : 0,
                    No_of_Discharges = dischargesByMonth.ContainsKey(month) ? dischargesByMonth[month] : 0
                })
                .ToList();

            return Ok(result);
        }

        /*
        [HttpGet("MostThreateningSubstance")]
        public async Task<ActionResult<string>> GetMostThreateningSubstance()
        {
            try
            {
                // Get all approved applications
                var approvedApplications = await _context.Applications
                    .Where(app => app.Status == "Approved")
                    .Include(app => app.ClientId)
                    .ThenInclude(client => client.Substances)
                    .ToListAsync();

                if (!approvedApplications.Any())
                {
                    return NotFound("No approved applications found");
                }

                // Count substance occurrences across all approved applications
                var substanceCounts = approvedApplications
                    .SelectMany(app => app.Client?.Substances ?? new List<Substance>())
                    .Where(substance => substance != null && !string.IsNullOrEmpty(substance.Name))
                    .GroupBy(substance => substance.Name)
                    .Select(group => new
                    {
                        SubstanceName = group.Key,
                        Count = group.Count(),
                        // You can also consider threat level based on DailyThreshold if needed
                        AverageThreshold = group.Average(s => s.DailyThreshold ?? 0)
                    })
                    .OrderByDescending(x => x.Count) // Sort by frequency first
                    .ThenByDescending(x => x.AverageThreshold) // Then by threat level
                    .ToList();

                if (!substanceCounts.Any())
                {
                    return NotFound("No substances found in approved applications");
                }

                // Return the most common/threatening substance
                var mostThreatening = substanceCounts.First();

                return Ok(new
                {
                    MostThreateningSubstance = mostThreatening.SubstanceName,
                    OccurrenceCount = mostThreatening.Count(),
                    AverageDailyThreshold = mostThreatening.AverageThreshold
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving most threatening substance: {ex.Message}");
            }
        }*/

        private static string Get_Month(string str)
        {
            var returnval = "";
            if (str == "01")
            {
                returnval = "January";

            }
            else if (str == "02")
            {
                returnval = "February";

            }
            else if (str == "03")
            {
                returnval = "March";
            }
            else if (str == "04")
            {
                returnval = "April";
            }
            else if (str == "05")
            {
                returnval = "May";
            }
            else if (str == "06")
            {
                returnval = "June";
            }
            else if (str == "07")
            {
                returnval = "July";

            }
            else if (str == "08")
            {
                returnval = "August";
            }
            else if (str == "09")
            {
                returnval = "September";
            }
            else if (str == "10")
            {
                returnval = "October";
            }
            else if (str == "11")
            {
                returnval = "November";
            }
            else if (str == "12")
            {
                returnval = "December";
            }

            return returnval;
        }
    }
}
