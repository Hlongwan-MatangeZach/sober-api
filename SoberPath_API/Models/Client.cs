using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Client:User
    {
        public string? Location { get; set; }
        
        public int? Social_WorkerId { get; set; }

        [JsonIgnore]
        public List<SessionBooking>? SessionBooking { get; set; }

        [JsonIgnore]

        public List<Session>? Sessions { get; set; }

        [JsonIgnore]

        public List<Substance>? Substances { get; set; }


        [JsonIgnore]
        public List<Next_of_Kin>? Next_Of_Kins { get; set; }

        [JsonIgnore]

        public List<Rehab_Admission>? Rehab_Admission { get; set; }

        [JsonIgnore]
        public List<Client_Assignment>? Client_Assignment { get; set; }

        [JsonIgnore]
        public List<Application>? Application { get; set; }

        [JsonIgnore]
        public Room? Room { get; set; }
        [JsonIgnore]
        public List<Records>? Records { get; set; }

        




        
    }
}
