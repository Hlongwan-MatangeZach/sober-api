using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Linq;

namespace SoberPath_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Socail_WorkerController(Sober_Context context) : ControllerBase
    {
        
        private readonly Sober_Context _context = context;

        [HttpGet("GetAssignedClients/{id}")]
        public async Task<ActionResult<Client>> GetAssignedClients(int id)
        {
            var clients = await _context.Clients.Where(cl => cl.Social_WorkerId == id).ToListAsync();
            if(clients==null)
            {
                return NotFound();

            }

            return Ok(clients);
        }

        [HttpGet("GetSessionsByUser/{userId}")]
        public async Task<ActionResult> GetSessionsByUser(int userId)
        {
            var sessions = await _context.Sessions
                .Where(cl => cl.ClientId == userId)
                .Select(cl => new
                {
                    sessionType = cl.Type,
                    Date = cl.Date,
                })
                .ToListAsync();

            if (sessions == null || !sessions.Any())
            {
                return NotFound();
            }

            return Ok(sessions);
        }

        [HttpPost("Schedule")]
        public async Task<ActionResult> Set_Schedule(Event booking)
        {
            if(booking==null)
            {
                return BadRequest();
            }
            _context.Events.Add(booking);
            await _context.SaveChangesAsync();
            return Ok(booking);
        }

        [HttpPost("Send_Application")]
        public async Task<ActionResult> Send_Application(Application application)
        {
            if(application==null)
            {  return BadRequest(); 
            }

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return Ok(application);
        }


        [HttpGet("applications/{socialWorkerId}/monthly-stats")]
        public IActionResult GetMonthlyApplicationStatsForSocialWorker(int socialWorkerId)
        {
            var applications = _context.Applications
                .Where(a => a.Date != null && a.Social_WorkerId == socialWorkerId)
                .AsEnumerable()
                .Select(a => new
                {
                    Date = DateTime.Parse(a.Date),
                    Status = a.Status?.ToLower()
                })
                .ToList();

            if (!applications.Any())
            {
                var emptyMonths = Enumerable.Range(1, 12)
                    .Select(m => new DateTime(DateTime.Now.Year, m, 1).ToString("MMM"))
                    .ToList();

                return Ok(new
                {
                    months = emptyMonths,
                    applications = Enumerable.Repeat(0, 12).ToList(),
                    accepted = Enumerable.Repeat(0, 12).ToList(),
                    rejected = Enumerable.Repeat(0, 12).ToList()
                });
            }

            var minDate = new DateTime(applications.Min(a => a.Date).Year, applications.Min(a => a.Date).Month, 1);
            var maxDate = new DateTime(applications.Max(a => a.Date).Year, applications.Max(a => a.Date).Month, 1);

            var monthsList = new List<DateTime>();
            for (var date = minDate; date <= maxDate; date = date.AddMonths(1))
            {
                monthsList.Add(date);
            }

            var grouped = applications
                .GroupBy(a => new DateTime(a.Date.Year, a.Date.Month, 1))
                .ToDictionary(g => g.Key, g => g.ToList());

            var months = new List<string>();
            var applicationsCount = new List<int>();
            var acceptedCount = new List<int>();
            var rejectedCount = new List<int>();

            foreach (var monthDate in monthsList)
            {
                months.Add(monthDate.ToString("MMM"));

                if (grouped.TryGetValue(monthDate, out var appsInMonth))
                {
                    applicationsCount.Add(appsInMonth.Count);
                    acceptedCount.Add(appsInMonth.Count(a => a.Status == "approved" || a.Status == "approved & allocated"));
                    rejectedCount.Add(appsInMonth.Count(a => a.Status == "rejected"));
                }
                else
                {
                    applicationsCount.Add(0);
                    acceptedCount.Add(0);
                    rejectedCount.Add(0);
                }
            }

            return Ok(new
            {
                months,
                applications = applicationsCount,
                accepted = acceptedCount,
                rejected = rejectedCount
            });
        }


        [HttpGet("GetApplications/{id}")]
        public async Task<ActionResult> GetApplications(int id)
        {
            var applications = await (from app in _context.Applications
                                      join client in _context.Clients
                                      on app.ClientId equals client.Id
                                      where app.Social_WorkerId == id
                                      select new
                                      {
                                          ApplicationStatus = app.Status,
                                          ClientId = client.Id,
                                          ClientName = client.Name,
                                          ClientSurname = client.Surname
                                      }).ToListAsync();

            if (applications == null || !applications.Any())
            {
                return NotFound();
            }

            return Ok(applications);
        }


        [HttpGet("substance-trends/{socialWorkerId}")]
        public async Task<IActionResult> GetSubstanceTrends(int socialWorkerId)
        {
            // Get all clients assigned to this social worker
            var clientIds = await _context.Clients
                .Where(c => c.Social_WorkerId == socialWorkerId)
                .Select(c => c.Id)
                .ToListAsync();

            if (!clientIds.Any())
            {
                return Ok(new
                {
                    substanceTrends = new
                    {
                        mostCommonSubstances = new List<object>()
                    }
                });
            }

            // Get all substances used by these clients
            var substances = await _context.Substances
                .Where(s => s.ClientId != null && clientIds.Contains(s.ClientId.Value))
                .ToListAsync();

            int total = substances.Count;
            if (total == 0)
            {
                return Ok(new
                {
                    substanceTrends = new
                    {
                        mostCommonSubstances = new List<object>()
                    }
                });
            }

            // Group substances by name and calculate counts and percentages
            var result = substances
                .GroupBy(s => s.Name)
                .Select(g => new
                {
                    substance = g.Key,
                    count = g.Count(),
                    percentage = Math.Round((double)g.Count() / total * 100, 2)
                })
                .OrderByDescending(g => g.count)
                .ToList();

            return Ok(new
            {
                substanceTrends = new
                {
                    mostCommonSubstances = result
                }
            });
        }


        [HttpGet("{socialWorkerId}/demographics")]
        public async Task<IActionResult> GetDemographics(int socialWorkerId)
        {
            var clients = await _context.Clients
                .Where(c => c.Social_WorkerId == socialWorkerId)
                .ToListAsync();

            if (!clients.Any())
            {
                return Ok(new
                {
                    demographics = new
                    {
                        ageGroups = new List<object>(),
                        race = new List<object>(),
                        gander=new List<object>()
                    }
                });
            }

            // --- AGE GROUPS ---
            var ageGroups = new Dictionary<string, int>
    {
        { "18-25", 0 },
        { "26-35", 0 },
        { "36-45", 0 },
        { "46-60", 0 },
        { "60+", 0 },
        { "Unknown", 0 }
    };

            foreach (var client in clients)
            {
                int? age = ParseAgeFromID(client.ID_Number);
                if (age == null)
                {
                    ageGroups["Unknown"]++;
                }
                else if (age <= 25)
                {
                    ageGroups["18-25"]++;
                }
                else if (age <= 35)
                {
                    ageGroups["26-35"]++;
                }
                else if (age <= 45)
                {
                    ageGroups["36-45"]++;
                }
                else if (age <= 60)
                {
                    ageGroups["46-60"]++;
                }
                else
                {
                    ageGroups["60+"]++;
                }
            }

            var ageGroupList = ageGroups
                .Where(kvp => kvp.Value > 0)
                .Select(kvp => new { group = kvp.Key, count = kvp.Value })
                .ToList();

            // --- RACE GROUP ---
            var raceList = clients
                .Where(c => !string.IsNullOrWhiteSpace(c.Race))
                .GroupBy(c => c.Race!.Trim())
                .Select(g => new { race = g.Key, count = g.Count() })
                .ToList();

            return Ok(new
            {
                demographics = new
                {
                    ageGroups = ageGroupList,
                    race = raceList
                }
            });
        }

        // --- Helper method to get age from SA ID Number ---
        private int? ParseAgeFromID(string? idNumber)
        {
            if (string.IsNullOrEmpty(idNumber) || idNumber.Length < 6)
                return null;

            try
            {
                string yearPart = idNumber.Substring(0, 2);
                string monthPart = idNumber.Substring(2, 2);
                string dayPart = idNumber.Substring(4, 2);

                int year = int.Parse(yearPart);
                int month = int.Parse(monthPart);
                int day = int.Parse(dayPart);

                // Determine century
                year += (year <= DateTime.UtcNow.Year % 100) ? 2000 : 1900;

                var birthDate = new DateTime(year, month, day);
                int age = DateTime.UtcNow.Year - birthDate.Year;
                if (birthDate > DateTime.UtcNow.AddYears(-age)) age--;

                return age;
            }
            catch
            {
                return null;
            }
        }

        [HttpGet("dashboard-stats/{socialWorkerId}")]
        public async Task<IActionResult> GetSocialWorkerStats(int socialWorkerId)
        {
            var assignedClients = await _context.Clients
                .Where(c => c.Social_WorkerId == socialWorkerId)
                .ToListAsync();

            var clientIds = assignedClients.Select(c => c.Id).ToList();

            var applications = await _context.Applications
                .Where(app => app.Social_WorkerId == socialWorkerId)
                .ToListAsync();

            var acceptedCount = applications.Count(a =>
                a.Status == "Approved" || a.Status == "Approved & Allocated");

            var declinedCount = applications.Count(a => a.Status == "Rejected");

            return Ok(new
            {
                socialWorkerStats = new
                {
                    totalAssignedClients = assignedClients.Count,
                    totalApplications = applications.Count,
                    acceptedApplications = acceptedCount,
                    declinedApplications = declinedCount
                }
            });
        }


        [HttpGet("clients/gender-stats/{socialWorkerId}")]
        public IActionResult GetGenderStatsForSocialWorker(int socialWorkerId)
        {
            var genderStats = _context.Clients
                .Where(c => c.Social_WorkerId == socialWorkerId)
                .GroupBy(c => c.Gender)
                .Select(g => new
                {
                    Gender = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .ToList();

            return Ok(genderStats);
        }

        [HttpGet("substance-stats/{socialWorkerId}")]
        public IActionResult GetSubstanceStats(int socialWorkerId)
        {
            // Get all substances used by clients assigned to this social worker
            var substances = _context.Substances
                .Select(s => s.Name)
                .Distinct()
                .ToList();

            // Prepare result lists
            var totalClients = new List<int>();
            var maleCounts = new List<int>();
            var femaleCounts = new List<int>();
            var otherGenderCounts = new List<int>();
            var indianCounts = new List<int>();
            var blackCounts = new List<int>();
            var whiteCounts = new List<int>();
            var coloredCounts = new List<int>();

            foreach (var substanceName in substances)
            {
                // Get clients for this substance + social worker
                var clients = _context.Clients
                    .Where(c => c.Social_WorkerId == socialWorkerId &&
                                c.Substances.Any(s => s.Name == substanceName))
                    .ToList();

                // Totals
                totalClients.Add(clients.Count);

                // Gender counts
                maleCounts.Add(clients.Count(c => c.Gender == "Male"));
                femaleCounts.Add(clients.Count(c => c.Gender == "Female"));
                otherGenderCounts.Add(clients.Count(c => c.Gender != "Male" && c.Gender != "Female"));

                // Race counts
                indianCounts.Add(clients.Count(c => c.Race == "Indian"));
                blackCounts.Add(clients.Count(c => c.Race == "Black"));
                whiteCounts.Add(clients.Count(c => c.Race == "White"));
                coloredCounts.Add(clients.Count(c => c.Race == "Colored"));
            }

            var result = new
            {
                Substance = substances,
                TotalClients = totalClients,
                Male = maleCounts,
                Female = femaleCounts,
                Others = otherGenderCounts,
                Indian = indianCounts,
                Black = blackCounts,
                White = whiteCounts,
                Colored = coloredCounts
            };

            return Ok(result);
        }


        [HttpGet("substance-stat/{socialWorkerId}")]
        public IActionResult GetSubstanceStat(int socialWorkerId)
        {
            // Fetch all substances for this social worker's clients
            var clientSubstances = _context.Clients
                .Where(c => c.Social_WorkerId == socialWorkerId)
                .Include(c => c.Substances) // Ensure substances are loaded
                .SelectMany(c => c.Substances.Select(s => new
                {
                    SubstanceName = s.Name,
                    Gender = c.Gender,
                    Race = c.Race
                }))
                .Where(x => !string.IsNullOrEmpty(x.SubstanceName))
                .ToList();

            // Get distinct substance names
            var allSubstances = clientSubstances
                .Select(x => x.SubstanceName)
                .Distinct()
                .ToList();

            // Group data
            var groupedData = allSubstances.Select(substanceName =>
            {
                var group = clientSubstances.Where(x => x.SubstanceName == substanceName);

                return new
                {
                    Substance = substanceName,
                    TotalClients = group.Count(),
                    Male = group.Count(x => x.Gender == "Male"),
                    Female = group.Count(x => x.Gender == "Female"),
                    Others = group.Count(x => x.Gender != "Male" && x.Gender != "Female"),
                    Indian = group.Count(x => x.Race == "Indian"),
                    Black = group.Count(x => x.Race == "Black"),
                    White = group.Count(x => x.Race == "White"),
                    Colored = group.Count(x => x.Race == "Colored")
                };
            }).ToList();

            // JSON response format
            var response = new
            {
                Substances = groupedData.Select(g => g.Substance).ToList(),
                TotalClients = groupedData.Select(g => g.TotalClients).ToList(),
                Male = groupedData.Select(g => g.Male).ToList(),
                Female = groupedData.Select(g => g.Female).ToList(),
                Others = groupedData.Select(g => g.Others).ToList(),
                Indian = groupedData.Select(g => g.Indian).ToList(),
                Black = groupedData.Select(g => g.Black).ToList(),
                White = groupedData.Select(g => g.White).ToList(),
                Colored = groupedData.Select(g => g.Colored).ToList()
            };

            return Ok(response);
        }

        [HttpGet("upcoming-sessions/{socialWorkerId}")]
        public async Task<IActionResult> GetUpcomingSessions(int socialWorkerId)
        {
            if (socialWorkerId <= 0)
                return BadRequest("Invalid social worker ID.");

            var today = DateTime.Today;
            var nextWeek = today.AddDays(7);

            var sessions = await _context.Events
                .Where(s => s.Social_Id == socialWorkerId
                            )
                .Select(
                       s => new
                       {
                           s.Id,
                           s.Client_Id,
                           ClientName = _context.Clients.Where(c => c.Id == s.Client_Id).Select(c => c.Name).FirstOrDefault(),
                           Assignment_Date = s.Date,
                           topic = s.Title

                       }).ToListAsync();
            return Ok(sessions);


        }

        [HttpGet("search-by-socialworker/{socialWorkerId}")]
        public async Task<IActionResult> SearchClientsBySocialWorker(int socialWorkerId, [FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty.");

            searchTerm = searchTerm.Trim().ToLower();

            var clients = await _context.Events
                .Where(sb => sb.Social_Id == socialWorkerId)
                .Join(
                    _context.Clients,
                    sb => sb.Client_Id,
                    c => c.Id,
                    (sb, c) => c
                )
                .Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(c.Surname) && c.Surname.ToLower().Contains(searchTerm))
                )
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Surname,
                    
                })
                .Distinct() // avoid duplicates if a client has multiple bookings
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(clients);
        }
    }
}

