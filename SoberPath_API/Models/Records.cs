namespace SoberPath_API.Models
{
    public class Records
    {
        public int Id { get; set; }
        public DateOnly? RecordedDate { get; set; }
        public double? Quantity { get; set; }
        public int? ClientId { get; set; }
        public int? SubstanceId { get; set; }

        // This is the navigation property
        public Substance? Substance { get; set; }

    }
}
