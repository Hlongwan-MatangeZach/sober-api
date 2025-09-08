using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    
    public class NGO_Admin:User
    {

        [JsonIgnore]
        public List<Client_Assignment>? Client_Assignments { get; set; }
        [JsonIgnore]

        public List<Event>? SessionBookings { get; set; }

    }
}
