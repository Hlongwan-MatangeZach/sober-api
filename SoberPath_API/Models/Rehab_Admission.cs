using System.ComponentModel.DataAnnotations;

namespace SoberPath_API.Models
{
    public class Rehab_Admission
    {

        [Key]
        public int Id { get; set; }

        public int? ClientId {  get; set; }

        public int? ApplicationId   { get; set; }

        public string? Admission_Date {  get; set; }

        public string? Expected_Dischanrge {  get; set; }

        public string? Discharged_Date { get; set; }

        public string? Dischange_status { get; set; }

        public bool? IsDischarged { get; set; }

    }
}
