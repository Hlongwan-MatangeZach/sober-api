using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace SoberPath_API.Models
{
    public class Social_Worker_Schedule
    {

        [Key]

        public int Id { get; set; }

        public string? Availabillity_Status { get; set; }

        public string? Notes { get; set; }

        public DateTime Date { get; set; }

        public int? Social_WorkerId { get; set; }

    }
}
