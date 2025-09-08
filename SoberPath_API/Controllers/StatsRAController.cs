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

        [HttpGet("Monthly_Admission_Data")]
        public async Task<ActionResult> GetMonthlyAdmission_Data()
        {
            var returnval = await _context.Applications.Where(app => app.Status == "Approved & Allocated" || app.Status == "Discharged" && app.ClientId != null && app.Date != null && app.Status_Update_Date != null).Select(app => new
            {

                Status = app.Status,
                Date_submitted = app.Date,
                Status_Update_Date = app.Status_Update_Date,
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
