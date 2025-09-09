namespace SoberPath_API.Models
{
    public class Event
    {
        public int Id { get; set; }
        //attributes 

        public string? Title { get; set; }
        public string? Date { get; set; }  
        public string? Description { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        //foreign keys 
        public int? Social_Id { get; set; }
        public int? Client_Id { get; set; }
        public int? NGO_Id { get; set; }

    }
}
