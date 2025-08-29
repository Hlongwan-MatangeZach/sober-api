using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Rehabilitation_Center
    {
        [Key]
        public int Id { get; set; }

        public int? Center_code { get; set; }

        public string? Center_Name { get; set; }

        public string? Capacity { get; set; }

        [JsonIgnore]
        public List<Rehab_Admin>? Rehab_Admins { get; set; }

    }
}
