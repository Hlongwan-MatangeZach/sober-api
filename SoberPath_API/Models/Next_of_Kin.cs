namespace SoberPath_API.Models
{
    public class Next_of_Kin
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Relationship { get; set; }

        public string? Phone_number  { get; set; }

        public int? ClientId    { get; set; }
        
    }
}
