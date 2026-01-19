namespace Booking_Hotel.Models.ViewModels
{
    public class BookingRoomVM
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        public string BranchName { get; set; }

        public string RoomType { get; set; } // Standard / Deluxe / Suite
    }
}
