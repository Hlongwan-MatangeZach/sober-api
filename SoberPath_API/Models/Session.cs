using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        public string? Date {  get; set; }

        public string? Type { get; set; }
        public TimeSpan? Duration   { get; set; }
        public string? Session_Note   { get; set; }


        public int? ClientId { get; set; }
        public int? Social_WorkerId { get; set; }
       
    }
}
