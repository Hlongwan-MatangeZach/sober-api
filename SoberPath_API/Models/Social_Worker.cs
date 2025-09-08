using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    
    public class Social_Worker:User
    {
        

        [JsonIgnore]
        public List<Application>? Applications { get; set; }

        [JsonIgnore]
        public List<Client_Assignment>? Client_Assignments { get; set; }
        [JsonIgnore]
        public List<Session>? Sessions { get; set; }

        [JsonIgnore]

        public List<Social_Worker_Schedule>? Social_Worker_Schedules { get; set; }
        [JsonIgnore]

        public List<Client>? Clients { get; set; }

        [JsonIgnore]
        public List<Event>? Events { get; set; }
    }
}
