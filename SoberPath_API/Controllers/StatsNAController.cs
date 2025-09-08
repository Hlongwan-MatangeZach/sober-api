using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Globalization;
using static SoberPath_API.Controllers.StatsController;

namespace SoberPath_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsNAController(Sober_Context context) : ControllerBase
    {
        private readonly Sober_Context _context = context;

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

       /* [HttpGet("LocationvsClients_GraphData")]
        public async Task<ActionResult> Get_LocationvsClients()
        {
            var retuurnal = await _context.Clients.Where(cl => cl.Location != null).GroupBy(cl => cl.Location).Where(g => g.Key != null).Select(group => new
            {
                location_ = group.Key,
                Number = _context.Clients.Where(cl => cl.Location == group.Key).Count(),

            }).ToListAsync();

            return Ok(retuurnal);

        }*/

        [HttpGet("Ave_No._Clients__per_SW")]
        public async Task<ActionResult> GetAverageNumber()
        {
            var returnval = await _context.Social_Workers.Select(sww => new { number = _context.Clients.Where(cl => cl.Social_WorkerId == sww.Id).Count() }).AverageAsync(sw => sw.number);



            return Ok(Math.Round(returnval, 2));
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
