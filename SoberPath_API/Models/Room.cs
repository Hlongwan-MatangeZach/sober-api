using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace SoberPath_API.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string? BuildingName { get; set; }
        public string? RoomNumber { get; set; }
        public string? AllocatedDate { get; set; }

        public bool? Occupying { get; set; }
        

        public  int? ClientId { get; set; }


    }
}
