using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class SessionBooking
    {

        [Key]
        public int Id { get; set; }


        public int? Social_WorkerId { get; set; }

        public int? NGO_AdminId { get; set; }

        public int? ClientId { get; set; }

        public string? Stype { get; set; }

        public int? Stime { get; set; }



        public string? Assignment_Date { get; set; }


    }
}
