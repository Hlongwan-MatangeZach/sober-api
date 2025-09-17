namespace SoberPath_API.Models
{
    public class ProgressDto
    {

        public int ClientId { get; set; }
        public double ProgressPercentage { get; set; }
        public int CompletedDays { get; set; }
        public int TotalDays { get; set; }
        public double AddictionRate { get; set; } // New field
        public string TimeframeDescription { get; set; }
        public List<SubstanceProgress> SubstanceProgressDetails { get; set; } = new();
    }
}
