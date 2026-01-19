namespace Booking_Hotel.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Thống kê tổng
        public int TotalUsers { get; set; }
        public int TotalRooms { get; set; }
        public int BookingsThisMonth { get; set; }
        public decimal TotalRevenue { get; set; }

        // Booking gần đây
        public List<RecentBookingVM> RecentBookings { get; set; }

        // Trạng thái phòng
        public int AvailableRooms { get; set; }
        public int BookedRooms { get; set; }
        public int MaintenanceRooms { get; set; }

        // Chi nhánh
        public List<BranchVM> Branches { get; set; }

        // User mới
        public List<UserVM> NewUsers { get; set; }
    }

    public class RecentBookingVM
    {
        public int BookingId { get; set; }
        public string FullName { get; set; }
        public string RoomName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
    }

    public class BranchVM
    {
        public string BranchName { get; set; }
        public int RoomCount { get; set; }
    }

    public class UserVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
