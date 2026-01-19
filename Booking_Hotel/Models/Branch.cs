using System.ComponentModel.DataAnnotations;

namespace Booking_Hotel.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Tên chi nhánh không được để trống")]
        [StringLength(150)]
        public string BranchName { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        // Navigation
        public ICollection<Room>? Rooms { get; set; }
    }
}
