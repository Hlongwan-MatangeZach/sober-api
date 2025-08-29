using System.ComponentModel.DataAnnotations;

namespace SoberPath_API.Models
{
    public class Recieved_Applications
    {

        [Key]
        public int Id { get; set; }

        public int? Rehab_AdminId   { get; set; }

        public int? ApplicationId { get; set; }

        public DateOnly? Processing_Date { get; set; }
    }
}
