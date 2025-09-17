using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Application
    {
        [Key]
        //main attributees 
        public int Id { get; set; }
        public string? Date { get; set; }
        public string? Summary { get; set; }
        public string? Status { get; set; }

        //foreign keys 
        public int? ClientId { get; set; }
        public int? Social_WorkerId { get; set; }
        public int? Rehab_AdminID { get; set; }

        //pdf things 
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }

        //processing stuffs
        public bool? IsRead { get; set; }
        public string? RejectionReason { get; set; }


        public string? Status_Update_Date { get; set; }
        public bool? HasRelapse { get; set; }

        [JsonIgnore]
        public Rehab_Disharge? Rehab_Disharge { get; set; }




    }
}
