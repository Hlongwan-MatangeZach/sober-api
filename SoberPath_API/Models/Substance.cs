using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Substance
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? ClientId { set; get; }
        public double? DailyThreshold { get; set; } // Critical for calculations
        public string? unit { get; set; }


        [JsonIgnore]
        public List<Records>? Records { get; set; }
        


    }
}
