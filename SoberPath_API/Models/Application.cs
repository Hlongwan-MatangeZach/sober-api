using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Application
    {
        [Key]
        public int Id { get; set; }
        public string? Date { get; set; }
        public string? Comments { get; set; }

        public string? Reason { get; set; }

        public string? Summary { get; set; }

        public string? Addiction_level { get; set; }

        public string? Substance_type { get; set; }

        public int? ClientId { get; set; }

        public int? Social_WorkerId { get; set; }

        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }

        public string? Status { get; set; }

        public string? Status_Update_Date { get; set; }

        public string? Update_Comment { get; set; }


        public string? Discharge_Date { get; set; }

        public string? Discharge_Reason { get; set; }

        public string? RehabReason { get; set; }
        public bool? HasRelapse { get; set; }



        [JsonIgnore]
        public List<Recieved_Applications>? Recieved_Applications { get; set; }

    }
}
