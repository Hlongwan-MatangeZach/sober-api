namespace SoberPath_API.Models
{
    public class ApplicationDetailsDto
    {
        public string? ClientName { get; set; }
        public string? ClientSurname { get; set; }
        public string? ClientPhone { get; set; }
        public string? ClientEmail { get; set; }
        public string? ClientAddress { get; set; }

        public string? SocialWorkerName { get; set; }
        public string? SocialWorkerSurname { get; set; }

        public string? RehabAdminName { get; set; }
        public string? RehabAdminSurname { get; set; }

        public string? RoomNumber { get; set; }
        public string? ApplicationStatus { get; set; }
        public string? AdmissionDate { get; set; }
        public string? DischargeDate { get; set; }
    }
}
