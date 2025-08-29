namespace SoberPath_API.Models
{
    public class SubstanceProgress
    {
        public int SubstanceId { get; set; }
        public string SubstanceName { get; set; }
        public int DaysWithinThreshold { get; set; }
        public int DaysExceededThreshold { get; set; }
    }
}
