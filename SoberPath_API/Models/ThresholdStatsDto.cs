namespace SoberPath_API.Models
{
    public class ThresholdStatsDto
    {
        public string SubstanceName { get; set; } = "";
        public string Unit { get; set; } = "";
        public int DaysExceedingThreshold { get; set; }
        public int TotalDays { get; set; }
        public double MaxOverThreshold { get; set; }
        public double PercentageSafeDays { get; set; }

        // Optional for display purposes
        public double? DailyThreshold { get; set; }
        public string? Description { get; set; }
    }
}
