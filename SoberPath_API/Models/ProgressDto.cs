namespace SoberPath_API.Models
{
    public class ProgressDto
    {
      
            public int ClientId { get; set; }
            public double ProgressPercentage { get; set; } // 0-100
            public int CompletedDays { get; set; } // Days without exceeding threshold
            public int TotalDays { get; set; } // Total days in the period
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        public List<SubstanceProgress> SubstanceProgressDetails { get; set; } = new();
    }
}
