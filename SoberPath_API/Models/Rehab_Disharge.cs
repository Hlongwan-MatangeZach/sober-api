using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Rehab_Disharge
    {
        [Key]
        public int Id { get; set; }
        public int? ApplicationId { get; set; }
        public string? Disharge_Date { get; set; }
        public string? Disharge_Reason { get;set; }

        

        
    }
}
