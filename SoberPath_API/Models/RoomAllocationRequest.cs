namespace SoberPath_API.Models
{
    public class RoomAllocationRequest
    {
        public int RoomId { get; set; }
        public int ClientId { get; set; }
        public DateTime? ExpectedCheckOutDate { get; set; }
    }
}
