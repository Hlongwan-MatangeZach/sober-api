namespace SoberPath_API.Models
{
    public class WeeklyStatsDto
    {
        public string SubstanceName { get; set; }
        public string Unit { get; set; }
        public List<string> WeekNumbers { get; set; } = new List<string>();
        public List<double> WeeklyTotals { get; set; } = new List<double>();
    }
}
