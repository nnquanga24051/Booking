namespace Booking_Hotel.Models.ViewModels
{
    public class AdminUserVM
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string? Password { get; set; }

        public string Role { get; set; }
    }
}
