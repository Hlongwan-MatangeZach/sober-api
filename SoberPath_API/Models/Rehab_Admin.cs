using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    
    public class Rehab_Admin:User
    {


        

        [JsonIgnore]

        public List<Recieved_Applications>? Recieved_Applications { get; set; }

        [JsonIgnore]
        public List<Rehab_Admission>? Rehab_Admissions { get; set; }

    }
}
