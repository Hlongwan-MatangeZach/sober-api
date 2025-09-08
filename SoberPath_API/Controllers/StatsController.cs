
///Stats Controoller
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

        [HttpGet("Applications_Trend_LineGraph")]
        public async Task<ActionResult> Get_ApplicationTrend_Data()
        {
            var returnval = await _context.Applications.Where(app => app.Status != null && app.Date != null && (app.Status == "Approved" || app.Status == "Rejected" || app.Status == "Approved & Allocated")).GroupBy(app => app.Status).Select(group => new
            {
                id = group.Key,
                data = group
                    .Select(app => new {
                        MonthNumber = app.Date!.Substring(5, 2)
                    })
                    .GroupBy(x => x.MonthNumber)
                    .Select(g => new
                    {
                        x = Get_Month(g.Key),

                        y = _context.Applications.Where(app => app.Status != null && app.Status == group.Key && app.Date != null && app.Date.Substring(5, 2).Equals(g.Key)).Count(),

                    })
                    .ToList()
            })
            .ToListAsync();


            return Ok(returnval);
        }

        [HttpGet("Ave_No_Admitted")]
        public async Task<ActionResult> GetAverageNumberAdmitted()
        {
            var returnval = await _context.Applications.Select(app => new { number = _context.Applications.Where(app => app.Status == "Approved & Allocated").Count() }).AverageAsync(app => app.number);



            return Ok(Math.Round(returnval, 2));
        }

        [HttpGet("Recovery_Trend_Data")]
        public async Task<ActionResult> GetRecoveryTrend_Data()
        {
            var returval = await _context.Applications.Where(app => app.Status == "Discharged" && app.Date != null).GroupBy(app => app.Date).Select(app => new {
                Number = _context.Applications.Where(app => app.Status == "Discharged").Count(),



            }).FirstOrDefaultAsync();
            return Ok();
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

        [HttpGet("ProcesscingTime_Data")]
        public async Task<ActionResult> GetProcessingTime_Data()
        {
            // Get approved applications with their dates
            var approvedApps = await _context.Applications
                .Where(app => app.Date != null
                        && app.Status_Update_Date != null
                        && (app.Status == "Approved" || app.Status == "Approved & Allocated"))
                .Select(app => new
                {
                    Id = app.Id,
                    CreateDate = app.Date,
                    UpdateDate = app.Status_Update_Date,
                    Status = app.Status
                })
                .ToListAsync();


            var result = approvedApps
                .Select(app =>
                {
                    var createDate = DateTime.Parse(app.CreateDate!);
                    var updateDate = DateTime.Parse(app.UpdateDate!);
                    var processingDays = (updateDate - createDate).TotalDays;

                    return new
                    {
                        Id = app.Id,
                        Status = app.Status,
                        CreateDate = app.CreateDate,
                        ProcessingDays = processingDays
                    };
                })
                .Where(x => x.ProcessingDays >= 0) // Only include valid time spans
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Data = g.Select(item => new
                    {
                        x = DateTime.Parse(item.CreateDate!),
                        y = item.ProcessingDays,
                        id = item.Id,
                        formattedDate = item.CreateDate,
                        status = item.Status
                    }).ToList()
                })
                .ToList();

            return Ok(result);
        }

        [HttpGet("IT")]
        public async Task<ActionResult> Data()
        {
            var returnval = await _context.Applications.Where(app => app.Date != null && app.Status_Update_Date != null && app.Status != null && (app.Status == "Approved" || app.Status == "Approved & Allocated" && app.Date != null && app.Status_Update_Date != null)).
                Select(app => new
                {
                    id = app.Id,
                    createddate = app.Date,
                    updateddate = app.Status_Update_Date,
                    status = app.Status,

                }).Select(newobj => new
                {
                    status_ = newobj.status,
                    Number = DateTime.Parse(newobj.updateddate!).Subtract(DateTime.Parse(newobj.createddate!)).TotalDays,
                    Createday = newobj.createddate,
                    id_ = newobj.id,


                }).GroupBy(newobj => newobj.status_).Select(g => new
                {
                    id = g.Key,
                    Data = g.Select(obj => new
                    {
                        y = obj.Number,
                        x = DateTime.Parse(obj.Createday!).Date,
                        id = obj.id_,
                        formatteddate = obj.Createday,
                        status = obj.status_,
                    })
                }).ToListAsync();



            if (returnval == null)
            {
                return BadRequest();

            }


            return Ok(returnval);

        }

    
        //NGO stat to diplay the total number of users(SW,Clients,RehabAdmins,NGOAdmins)
        [HttpGet("Totals")]
        public async Task<ActionResult> GetTotals()
        {
            var totalClients = await _context.Clients.CountAsync();
            var totalSocialWorkers = await _context.Social_Workers.CountAsync(); // adjust entity name if different
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

        public class SubstanceCountDto
        {
            public string Name { get; set; } = "";
            public int Count { get; set; }
        }

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