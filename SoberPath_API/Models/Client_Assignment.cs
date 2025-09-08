using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SoberPath_API.Models
{
    public class Client_Assignment
    {

        [Key]
        public int Id { get; set; }

        //foreign keys 
        public int? Social_WorkerId {  get; set; }

        public int? NGO_AdminId { get; set; }

        public int? ClientId { get; set; }

        //attributes 
        public DateOnly? Assignment_Date { get; set; }










    }
}
