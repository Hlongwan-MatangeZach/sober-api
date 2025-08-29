using System.ComponentModel.DataAnnotations;

namespace SoberPath_API.Models
{
    public class Rehabilitation_Progress
    {

        [Key]
        public int Id { set; get; }

        public string? Progress { set; get; }
        public string? Date { set; get; }

        public int? ClientId { set; get; }
    }
}
