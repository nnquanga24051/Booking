using System.ComponentModel.DataAnnotations;

namespace Booking_Hotel.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required]
        public string BranchName { get; set; }

        public string Address { get; set; }
    }
}
