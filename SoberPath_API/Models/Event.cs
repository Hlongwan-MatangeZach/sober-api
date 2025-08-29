namespace SoberPath_API.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Date { get; set; }  
        public string? Description { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public int Social_Id { get; set; }
    }
}
