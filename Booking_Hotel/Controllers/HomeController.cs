using Microsoft.AspNetCore.Mvc;
using Booking_Hotel.Data;
using Booking_Hotel.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking_Hotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Booking(string branch, string roomType, DateTime? checkin, DateTime? checkout,
            decimal? minPrice, decimal? maxPrice, string[] status)
        {
            var rooms = _context.Rooms.Include(r => r.Branch).AsQueryable();

            // Lọc theo chi nhánh
            if (!string.IsNullOrEmpty(branch))
            {
                if (int.TryParse(branch, out int branchId))
                {
                    rooms = rooms.Where(r => r.BranchId == branchId);
                }
            }

            // Lọc theo loại phòng
            if (!string.IsNullOrEmpty(roomType))
            {
                rooms = rooms.Where(r => r.RoomType == roomType);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                rooms = rooms.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                rooms = rooms.Where(r => r.Price <= maxPrice.Value);
            }

            // Lọc theo trạng thái
            if (status != null && status.Length > 0)
            {
                var statusList = new List<string>();
                foreach (var s in status)
                {
                    if (s == "available")
                        statusList.Add("Available");
                    else if (s == "maintenance")
                        statusList.Add("Maintenance");
                }

                if (statusList.Count > 0)
                {
                    rooms = rooms.Where(r => statusList.Contains(r.Status));
                }
            }

            // Lọc theo ngày available (nếu có checkin/checkout)
            if (checkin.HasValue && checkout.HasValue)
            {
                // Lấy các phòng không bị booking trong khoảng thời gian này
                var bookedRoomIds = _context.Bookings
                    .Where(b => b.Status != "Cancelled")
                    .Where(b => (checkin.Value >= b.CheckIn && checkin.Value < b.CheckOut) ||
                               (checkout.Value > b.CheckIn && checkout.Value <= b.CheckOut) ||
                               (checkin.Value <= b.CheckIn && checkout.Value >= b.CheckOut))
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToList();

                rooms = rooms.Where(r => !bookedRoomIds.Contains(r.RoomId));
            }

            // Truyền thông tin để giữ lại giá trị filter
            ViewBag.Branches = _context.Branches.ToList();
            ViewBag.SelectedBranch = branch;
            ViewBag.SelectedRoomType = roomType;
            ViewBag.SelectedCheckin = checkin?.ToString("yyyy-MM-dd");
            ViewBag.SelectedCheckout = checkout?.ToString("yyyy-MM-dd");
            ViewBag.SelectedMinPrice = minPrice;
            ViewBag.SelectedMaxPrice = maxPrice;
            ViewBag.SelectedStatus = status;

            return View(rooms.ToList());
        }

        public IActionResult BookingDetails(int roomId)
        {
            // Kiểm tra đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
                return RedirectToAction("Login", "Authentications");
            }

            var room = _context.Rooms.Include(r => r.Branch).FirstOrDefault(r => r.RoomId == roomId);
            if (room == null)
                return NotFound();

            // Truyền userId vào ViewBag để tự động điền
            ViewBag.UserId = userId.Value;
            return View(room);
        }

        [HttpPost]
        public IActionResult ConfirmBooking(int roomId, int userId, DateTime checkIn, DateTime checkOut)
        {
            // Kiểm tra đăng nhập
            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            if (sessionUserId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt phòng!";
                return RedirectToAction("Login", "Authentications");
            }

            // Đảm bảo userId từ form khớp với session (bảo mật)
            if (userId != sessionUserId.Value)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ!";
                return RedirectToAction("Index");
            }

            // Kiểm tra ngày hợp lệ
            if (checkIn >= checkOut)
            {
                TempData["ErrorMessage"] = "Ngày trả phòng phải sau ngày nhận phòng!";
                return RedirectToAction("BookingDetails", new { roomId = roomId });
            }

            if (checkIn < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Ngày nhận phòng không được là ngày quá khứ!";
                return RedirectToAction("BookingDetails", new { roomId = roomId });
            }

            // Kiểm tra phòng có tồn tại không
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == roomId);
            if (room == null)
            {
                TempData["ErrorMessage"] = "Phòng không tồn tại!";
                return RedirectToAction("Booking");
            }

            // Kiểm tra phòng có trống không
            var existingBooking = _context.Bookings
                .Where(b => b.RoomId == roomId && b.Status != "Cancelled")
                .Where(b => (checkIn >= b.CheckIn && checkIn < b.CheckOut) ||
                           (checkOut > b.CheckIn && checkOut <= b.CheckOut) ||
                           (checkIn <= b.CheckIn && checkOut >= b.CheckOut))
                .FirstOrDefault();

            if (existingBooking != null)
            {
                TempData["ErrorMessage"] = "Phòng đã được đặt trong khoảng thời gian này!";
                return RedirectToAction("BookingDetails", new { roomId = roomId });
            }

            // Tạo booking mới
            var booking = new Models.Booking
            {
                RoomId = roomId,
                UserId = userId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đặt phòng thành công!";
            return RedirectToAction("BookingSuccess", new { bookingId = booking.BookingId });
        }

        public IActionResult BookingSuccess(int bookingId)
        {
            // Kiểm tra đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentications");
            }

            var booking = _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Branch)
                .Include(b => b.User)
                .FirstOrDefault(b => b.BookingId == bookingId);

            // Đảm bảo chỉ user đã đặt mới xem được
            if (booking == null || booking.UserId != userId.Value)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt phòng!";
                return RedirectToAction("Index");
            }

            return View(booking);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}