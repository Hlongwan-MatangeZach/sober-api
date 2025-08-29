
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


        [HttpGet("Clients_Gender_Stats")]

        public async Task<ActionResult> Get_Client_Stats_byGender()
        {
            var sumclients = await _context.Clients.CountAsync();
            var sum_males = await _context.Clients.Where(cl => cl.Gender != null && cl.Gender.ToLower().Equals("male")).CountAsync();
            var sum_females = await _context.Clients.Where(cl => cl.Gender != null && cl.Gender.ToLower().Equals("female")).CountAsync();
            var sum_other = await _context.Clients.Where(cl => cl.Gender != null && cl.Gender.ToLower().Equals("other")).CountAsync();


            double female_percentage = Math.Round(((double)sum_females / (double)sumclients) * 100);
            double male_percentage = Math.Round(((double)sum_males / (double)sumclients) * 100);
            double other_percentage = Math.Round(((double)sum_other / (double)sumclients) * 100);

            var returnval = await _context.Clients.Where(cl => cl.Gender != null).GroupBy(cl => cl.Gender).Where(g => g.Key != null).Select(g => new
            {
                id = g.Key!.ToLower(),
                label = g.Key.ToLower(),
                value = g.Key.ToLower() == "male" ? male_percentage : g.Key.ToLower() == "female" ? female_percentage : other_percentage,
                color = g.Key.ToLower() == "male" ? "hsl(104, 70%, 50%)" : g.Key.ToLower() == "female" ? "hsl(162, 70%, 50%)" : "hsla(162, 62%, 29%, 1.00)",

            }).ToListAsync();



            return Ok(returnval);


        }

        [HttpGet("Clients_Race_Stats")]

        public async Task<ActionResult> Get_Client_RaceStats()
        {
            var sumclients = await _context.Clients.CountAsync();
            var sum_indian = await _context.Clients.Where(cl => cl.Race != null && cl.Race.ToLower().Equals("indian")).CountAsync();
            var sum_white = await _context.Clients.Where(cl => cl.Race != null && cl.Race.ToLower().Equals("white")).CountAsync();
            var sum_black = await _context.Clients.Where(cl => cl.Race != null && cl.Race.ToLower().Equals("black")).CountAsync();
            var sum_coloured = await _context.Clients.Where(cl => cl.Race != null && cl.Race.ToLower().Equals("coloured")).CountAsync();
            var sum_other = await _context.Clients.Where(cl => cl.Race != null && cl.Race.ToLower().Equals("other")).CountAsync();


            double indian_percentage = Math.Round(((double)sum_indian / (double)sumclients) * 100);
            double black_percentage = Math.Round(((double)sum_black / (double)sumclients) * 100);
            double white_percentage = Math.Round(((double)sum_white / (double)sumclients) * 100);
            double coloured_percentage = Math.Round(((double)sum_coloured / (double)sumclients) * 100);
            double other_percentage = Math.Round(((double)sum_other / (double)sumclients) * 100);


            var returnval = await _context.Clients.Where(cl => cl.Race != null).GroupBy(cl => cl.Race).Where(g => g.Key != null).Select(g => new
            {
                id = g.Key!.ToLower(),
                label = g.Key.ToLower(),
                value = g.Key.ToLower() == "indian" ? indian_percentage : g.Key.ToLower() == "white" ? white_percentage : g.Key.ToLower() == "black" ? black_percentage : g.Key.ToLower() == "coloured" ? coloured_percentage : other_percentage,
                color = g.Key.ToLower() == "indian" ? "#3f51b5" : g.Key.ToLower() == "white" ? "#f50057" : g.Key.ToLower() == "black" ? "hsl(291, 70%, 50%)" : g.Key.ToLower() == "coloured" ? "hsl(157, 70%, 50%)" : "hsl(331, 70%, 50%)",

            }).ToListAsync();



            return Ok(returnval);


        }

        [HttpGet("Gender_percentage")]

        public async Task<ActionResult> Get_GenderPercentage()
        {
            var sumclients = await _context.Applications.Where(app => app.Status == "Approved & Allocated" || app.Status == "Approved").CountAsync();

            var sum_males = await _context.Applications.Where(app => app.Status == "Approved & Allocated" || app.Status == "Approved" &&
            _context.Clients.Any(client =>
            client.Id == app.ClientId && client.Gender != null &&
            client.Gender.ToLower().Equals("male")

            )).CountAsync();
            var sum_females = await _context.Applications.Where(app => app.Status == "Approved & Allocated" || app.Status == "Approved" && _context.Clients.Any(client =>
            client.Id == app.ClientId && client.Gender != null &&
            client.Gender.ToLower().Equals("female")

            )).CountAsync();
            var sum_other = await _context.Applications.Where(app => app.Status == "Approved & Allocated" || app.Status == "Approved" && _context.Clients.Any(client =>
            client.Id == app.ClientId && client.Gender != null &&
            client.Gender.ToLower().Equals("other")

            )).CountAsync();


            double female_percentage = Math.Round(((double)sum_females / (double)sumclients) * 100);
            double male_percentage = Math.Round(((double)sum_males / (double)sumclients) * 100);
            double other_percentage = Math.Round(((double)sum_other / (double)sumclients) * 100);

            var returnval = await _context.Clients.Where(cl => cl.Gender != null).GroupBy(cl => cl.Gender).Where(g => g.Key != null).Select(g => new
            {
                id = g.Key!.ToLower(),
                label = g.Key.ToLower(),
                value = g.Key.ToLower() == "male" ? male_percentage : g.Key.ToLower() == "female" ? female_percentage : other_percentage,
                color = g.Key.ToLower() == "male" ? "hsl(104, 70%, 50%)" : g.Key.ToLower() == "female" ? "hsl(162, 70%, 50%)" : "hsla(162, 62%, 29%, 1.00)",

            }).ToListAsync();



            return Ok(returnval);


        }



        [HttpGet("SWvsClienst_GraphData")]
        public async Task<ActionResult> Get_SWvsClients()
        {
            var retuurnal = await _context.Social_Workers.Select(sw => new
            {
                social_worker = sw.Name,
                No_of_clients = _context.Clients.Where(cl => cl.Social_WorkerId != null && cl.Social_WorkerId == sw.Id).Count(),


            }).ToListAsync();

            return Ok(retuurnal);

        }

        [HttpGet("LocationvsClients_GraphData")]
        public async Task<ActionResult> Get_LocationvsClients()
        {
            var retuurnal = await _context.Clients.Where(cl => cl.Location != null).GroupBy(cl => cl.Location).Where(g => g.Key != null).Select(group => new
            {
                location_ = group.Key,
                Number = _context.Clients.Where(cl => cl.Location == group.Key).Count(),

            }).ToListAsync();

            return Ok(retuurnal);

        }


        [HttpGet("Ave_No._Clients__per_SW")]

        public async Task<ActionResult> GetAverageNumber()
        {
            var returnval = await _context.Social_Workers.Select(sww => new { number = _context.Clients.Where(cl => cl.Social_WorkerId == sww.Id).Count() }).AverageAsync(sw => sw.number);



            return Ok(Math.Round(returnval, 2));
        }


        [HttpGet("Ave_No_Admitted")]

        public async Task<ActionResult> GetAverageNumberAdmitted()
        {
            var returnval = await _context.Applications.Select(app => new { number = _context.Applications.Where(app => app.Status == "Approved & Allocated").Count() }).AverageAsync(app => app.number);



            return Ok(Math.Round(returnval, 2));
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
            .ToListAsync(); // Now this works!


            return Ok(returnval);
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

        [HttpGet("DownloadFile/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null || application.Data == null)
                return NotFound("File not found");

            return File(application.Data, application.ContentType ?? "application/octet-stream", application.FileName);
        }


        [HttpPost("CreateApplication")]
        public async Task<IActionResult> CreateApplication([FromForm] IFormFile? file,
        [FromForm] string? date,
        [FromForm] string? comments,
        [FromForm] string? reason,
        [FromForm] string? summary,
        [FromForm] string? addiction_level,
        [FromForm] string? substance_type,
        [FromForm] int? clientId,
        [FromForm] int? social_workerId)
        {
            var application = new Application
            {
                Date = date,
                Comments = comments,
                Reason = reason,
                Summary = summary,
                Addiction_level = addiction_level,
                Substance_type = substance_type,
                ClientId = clientId,
                Social_WorkerId = social_workerId,
                Status = "Pending",
                HasRelapse = false
            };

            if (file != null && file.Length > 0)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                application.FileName = file.FileName;
                application.ContentType = file.ContentType;
                application.Data = ms.ToArray();
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new { application.Id, application.FileName, application.Status });
        }


        [HttpGet("GetMonthlyEvents/{socialId}/{month}/{year}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByMonth(int socialId, int month, int year)
        {
            var startDate = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            var endDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            return await _context.Events
                .Where(e => e.Social_Id == socialId &&
                       string.Compare(e.Date, startDate) >= 0 &&
                       string.Compare(e.Date, endDate) <= 0)
                .ToListAsync();
        }

        [HttpGet("GetDayEvents/{socialId}/{date}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByDate(int socialId, string date)
        {
            try
            {
                // Validate date format
                if (!DateTime.TryParse(date, out DateTime parsedDate))
                {
                    return BadRequest("Invalid date format. Use yyyy-MM-dd");
                }

                var events = await _context.Events
                    .Where(e => e.Social_Id == socialId && e.Date == date)
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("CreateEvent")]
        public async Task<ActionResult<Event>> CreateEvent(Event calendarEvent)
        {
            // Validate date format
            if (!DateTime.TryParse(calendarEvent.Date, out _))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd");
            }

            // Validate time formats
            if (!TimeSpan.TryParse(calendarEvent.StartTime, out _) ||
                !TimeSpan.TryParse(calendarEvent.EndTime, out _))
            {
                return BadRequest("Invalid time format. Use HH:mm");
            }

            _context.Events.Add(calendarEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = calendarEvent.Id }, calendarEvent);
        }

        [HttpDelete("DeleteEvent/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var calendarEvent = await _context.Events.FindAsync(id);
            if (calendarEvent == null)
            {
                return NotFound();
            }

            _context.Events.Remove(calendarEvent);
            await _context.SaveChangesAsync();

            return NoContent();
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

        [HttpGet("Processing_Trend_Data")]

        public async Task<ActionResult> Get_ProcessingTrend()
        {

            var returnval = await _context.Applications.Where(app_ => app_.Status_Update_Date != null && app_.Date != null && (app_.Status == "Approved" || app_.Status == "Pending" || app_.Status == "Rejected" || app_.Status == "Approved & Allocated" && app_.Status_Update_Date != null && app_.Date != null)).Select(app => new
            {
                Status = app.Status,
                Creation_date = app.Date,
                Update_date = app.Status_Update_Date,

            }).ToListAsync();

            var result = returnval.GroupBy(obj => obj.Status).Select(g => new
            {
                id = g.Key,
                data = g.Select(obj => new
                {
                    month_num = obj.Creation_date.Substring(5, 2),
                    Count_number = DateTime.Parse(obj.Update_date).Subtract(DateTime.Parse(obj.Creation_date)).TotalDays,



                }).GroupBy(obj => obj.month_num).Select(g => new
                {
                    x = Get_Month(g.Key),
                    y = g.Average(obj => obj.Count_number)
                }).ToList(),
            }).ToList();






            return Ok(result);

        }


        [HttpGet("Pending_Applications")]
        public async Task<ActionResult> Get_pendingapplication()
        {
            var returnval = await _context.Applications.Where(app => app.ClientId != null && app.Status == "Pending").CountAsync();
            return Ok(returnval);
        }


        [HttpGet("Monthly_Admission_Data")]

        public async Task<ActionResult> GetMonthlyAdmission_Data()
        {
            var returnval = await _context.Applications.Where(app => app.Status == "Approved & Allocated" || app.Status == "Discharged" && app.ClientId != null && app.Date != null && app.Status_Update_Date != null).Select(app => new
            {

                Status = app.Status,
                Date_submitted = app.Date,
                Status_Update_Date = app.Status_Update_Date,
                Discharge_Date = app.Discharge_Date,
                No_admissions = _context.Applications.Where(app => app.Status == "Approved & Allocated").Count(),
                No_Discharges = _context.Applications.Where(app => app.Status == "Approved & Allocated").Count(),


            }).ToListAsync();

            //string[] months = { "Januanry", "February", "March" ,"April","May","June","July","August","September","October","November","December" };

            var result = returnval.GroupBy(obj => obj.Date_submitted!.Substring(5, 2)).Select(g => new {
                Month = Get_Month(g.Key),
                No_of_Admissions = g.Where(obj_ => obj_.Status_Update_Date!.Substring(5, 2).Equals(g.Key) && obj_.Status == "Approved & Allocated").Count(),
                No_of_discharges = g.Where(obj_ => obj_.Status_Update_Date!.Substring(5, 2).Equals(g.Key) && obj_.Status == "Discharged").Count(),

            }).ToList();




            return Ok(result);

        }

        [HttpGet("Get_curent_month_applications")]

        public async Task<ActionResult> Get_current_month_applications()
        {
            var returnval = await _context.Applications.Where(app => app.ClientId != null && app.Status != null && app.Date != null).Select(app => new
            {
                Date = app.Date,
            }).ToListAsync();


            var result = returnval.Where(app => app.Date != null && DateTime.Parse(app.Date).Month == DateTime.Now.Month).Count();

            return Ok(result);
        }



        [HttpGet("Get_current_approvals")]
        public async Task<ActionResult> GetCurrent_approvals()
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
        //NGO for the most used substances
        [HttpGet("MostUsedSubstances")]
        public async Task<ActionResult<IEnumerable<SubstanceCountDto>>> GetMostUsedSubstances(int top = 1, bool includeTies = false)
        {
            top = Math.Max(1, Math.Min(100, top));

            // Group by normalized name (trim + lower) to collapse variants
            var groupedQuery = _context.Substances
                .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                .GroupBy(s => EF.Functions.Like(s.Name, "%")
                               ? s.Name!.Trim().ToLower()
                               : s.Name!.Trim().ToLower()) // explicit to ensure translatable expression
                .Select(g => new SubstanceCountDto
                {
                    Name = g.Key!,
                    Count = g.Count()
                });

            // fetch top N ordered by count desc
            var topList = await groupedQuery
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .Take(top)
                .AsNoTracking()
                .ToListAsync();

            if (!includeTies)
            {
                return Ok(topList);
            }

            // includeTies==true: return all substances that have count == maxCount (useful when top==1)
            if (topList.Count == 0)
                return Ok(topList);

            var maxCount = topList.Max(t => t.Count);

            var tied = await groupedQuery
                .Where(g => g.Count >= maxCount) // we only need those equal to maxCount, but >= is safe
                .OrderByDescending(g => g.Count)
                .ThenBy(g => g.Name)
                .AsNoTracking()
                .ToListAsync();

            // filter exactly equals maxCount (defensive)
            tied = tied.Where(t => t.Count == maxCount).ToList();

            return Ok(tied);
        }

    }
}