using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    
    public class User
    {
        [Key]
        public int Id  { get; set; }
        public string? Name { get; set; }
        public string? Surname   { get; set; }
        public string? ID_Number { get; set; }

        public string? Race { get; set; }

        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? Phone_Number { get; set; }

        public string? EmailAddress { get; set; }
        public string? Password { get; set; }

        



    }
}
