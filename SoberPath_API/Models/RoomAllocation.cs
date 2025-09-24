using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoberPath_API.Models
{
    public class RoomAllocation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime AllocationDate { get; set; } = DateTime.Now;

        public DateTime? ExpectedCheckOutDate { get; set; }

        public DateTime? ActualCheckOutDate { get; set; }

        public bool IsActive => ActualCheckOutDate == null;

        // Navigation properties
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }


    }
}
