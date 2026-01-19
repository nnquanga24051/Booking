using Booking_Hotel.Data;
using Booking_Hotel.Models;
using Booking_Hotel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace Booking_Hotel.Controllers
{
    [IgnoreAntiforgeryToken]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;


        public AdminController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();


        }

        // =========================
        // DASHBOARD
        // =========================
        public async Task<IActionResult> Dashboard()
        {
            // Check quyền Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Authentications");
            }

            var model = new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalRooms = await _context.Rooms.CountAsync(),

                BookingsThisMonth = await _context.Bookings
                    .CountAsync(b => b.CheckIn.Month == DateTime.Now.Month
                                  && b.CheckIn.Year == DateTime.Now.Year),

                TotalRevenue = await _context.Bookings
                    .Where(b => b.Status == "Completed")
                    .Join(_context.Rooms,
                        b => b.RoomId,
                        r => r.RoomId,
                        (b, r) => r.Price)
                    .SumAsync(),

                AvailableRooms = await _context.Rooms.CountAsync(r => r.Status == "Available"),
                BookedRooms = await _context.Rooms.CountAsync(r => r.Status == "Booked"),
                MaintenanceRooms = await _context.Rooms.CountAsync(r => r.Status == "Maintenance"),

                RecentBookings = await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Room)
                    .OrderByDescending(b => b.BookingId)
                    .Take(5)
                    .Select(b => new RecentBookingVM
                    {
                        BookingId = b.BookingId,
                        FullName = b.User.FullName,
                        RoomName = b.Room.RoomName,
                        CheckIn = b.CheckIn,
                        CheckOut = b.CheckOut,
                        Status = b.Status,
                        Price = b.Room.Price
                    }).ToListAsync(),

                Branches = await _context.Branches
                    .Select(br => new BranchVM
                    {
                        BranchName = br.BranchName,
                        RoomCount = _context.Rooms.Count(r => r.BranchId == br.BranchId)
                    }).ToListAsync(),

                NewUsers = await _context.Users
                    .OrderByDescending(u => u.UserId)
                    .Take(5)
                    .Select(u => new UserVM
                    {
                        FullName = u.FullName,
                        Email = u.Email
                    }).ToListAsync()
            };

            return View(model);
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }



        // =========================
        // USERS
        // =========================
      
        // =========================
        public async Task<IActionResult> Users(string? keyword)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentications");

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u =>
                    u.FullName.Contains(keyword) ||
                    u.Email.Contains(keyword));
            }

            var users = await query
                .OrderByDescending(u => u.UserId)
                .Select(u => new AdminUserVM
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Select(u => new AdminUserVM
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return Json(user);
        }


        // =========================
        // ADD USER
        // =========================

        //[HttpPost]
        //public async Task<IActionResult> AddUser([FromBody] AdminUserVM model)
        //{
        //    if (!IsAdmin()) return Unauthorized();

        //    if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        //        return BadRequest("Email đã tồn tại");

        //    var user = new User
        //    {
        //        FullName = model.FullName,
        //        Email = model.Email,
        //        Password = HashPassword(model.Password),
        //        Role = model.Role
        //    };

        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();
        //    return Ok();
        //}
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] AdminUserVM model)
        {
            if (!IsAdmin()) return Unauthorized();

            if (string.IsNullOrEmpty(model.FullName) ||
                string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin");
            }

            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest("Email đã tồn tại");
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Role = model.Role
            };

            // ✅ HASH CHUẨN
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Thêm người dùng thành công");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] AdminUserVM model)
        {
            if (!IsAdmin()) return Unauthorized();

            if (string.IsNullOrEmpty(model.FullName) ||
                string.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Vui lòng nhập đầy đủ thông tin");
            }

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
                return NotFound("Người dùng không tồn tại");

            // Check trùng email (ngoại trừ chính nó)
            if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.UserId != model.UserId))
            {
                return BadRequest("Email đã tồn tại");
            }

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.Password = _passwordHasher.HashPassword(user, model.Password);
            }


            await _context.SaveChangesAsync();

            return Ok("Cập nhật người dùng thành công");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            if (user.Role == "Admin")
                return BadRequest("Không thể xóa Admin");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }




        // =========================
        // PASSWORD HASH
        // =========================
        //private string HashPassword(string password)
        //{
        //    using var sha256 = System.Security.Cryptography.SHA256.Create();
        //    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //    return Convert.ToBase64String(bytes);
        //}

        // =========================
        // BRANCHES
        // =========================
        // ====== QUẢN LÝ CHI NHÁNH ======
        public async Task<IActionResult> Branches()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentications");

            var branches = await _context.Branches
                .OrderByDescending(b => b.BranchId)
                .ToListAsync();

            return View(branches);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] Branch model)
        {
            if (!IsAdmin())
                return Unauthorized("Không có quyền");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Branches.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }



        [HttpGet]
        public async Task<IActionResult> GetBranch(int id)
        {
            if (!IsAdmin())
                return Unauthorized();

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();

            return Ok(branch);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBranch([FromBody] Branch model)
        {
            if (!IsAdmin())
                return Unauthorized();

            var branch = await _context.Branches.FindAsync(model.BranchId);
            if (branch == null)
                return NotFound();

            branch.BranchName = model.BranchName;
            branch.Address = model.Address;

            await _context.SaveChangesAsync();
            return Ok(branch);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            //if (!IsAdmin())
            //    return Unauthorized();

            var branch = await _context.Branches.FindAsync(id);
            if (branch == null)
                return NotFound();

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // =========================
        // ROOMS
        // =========================
        public async Task<IActionResult> Rooms(
            string? keyword,
            int? branchId,
            string? roomType,
            string? status)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentications");

            var query = _context.Rooms
                .Include(r => r.Branch)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(r => r.RoomName.Contains(keyword));

            if (branchId.HasValue)
                query = query.Where(r => r.BranchId == branchId);

            if (!string.IsNullOrEmpty(roomType))
                query = query.Where(r => r.RoomType == roomType);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status == status);

            ViewBag.Branches = await _context.Branches.ToListAsync();
            ViewBag.RoomTypes = await _context.Rooms
                .Select(r => r.RoomType)
                .Distinct()
                .Where(x => x != null)
                .ToListAsync();

            var rooms = await query
                .OrderByDescending(r => r.RoomId)
                .ToListAsync();

            return View(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> GetRoom(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var room = await _context.Rooms
                .Include(r => r.Branch)
                .Where(r => r.RoomId == id)
                .Select(r => new
                {
                    r.RoomId,
                    r.RoomName,
                    r.Price,
                    r.Capacity,
                    r.Status,
                    r.RoomType,
                    r.ImageUrl,
                    r.BranchId
                })
                .FirstOrDefaultAsync();

            if (room == null)
                return NotFound();

            return Ok(room);
        }
       
        [HttpPost]
        public async Task<IActionResult> UpdateRoom([FromBody] Room model)
        {
            if (!IsAdmin()) return Unauthorized();

            var room = await _context.Rooms.FindAsync(model.RoomId);
            if (room == null)
                return NotFound("Phòng không tồn tại");

            if (!await _context.Branches.AnyAsync(b => b.BranchId == model.BranchId))
                return BadRequest("Chi nhánh không tồn tại");

            room.RoomName = model.RoomName;
            room.Price = model.Price;
            room.Capacity = model.Capacity;
            room.Status = model.Status;
            room.RoomType = model.RoomType;
            room.ImageUrl = model.ImageUrl;
            room.BranchId = model.BranchId;
            room.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok("Cập nhật phòng thành công");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
                return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            if (!IsAdmin()) return Unauthorized();

            var branches = await _context.Branches
                .Select(b => new
                {
                    b.BranchId,
                    b.BranchName
                })
                .ToListAsync();

            return Ok(branches);
        }
        [HttpPost]
        public async Task<IActionResult> SaveRoom([FromBody] Room model)
        {
            if (!IsAdmin()) return Unauthorized();

            if (string.IsNullOrEmpty(model.RoomName))
                return BadRequest("Tên phòng không được trống");

            if (model.RoomId == 0)
            {
                model.CreatedAt = DateTime.Now;
                _context.Rooms.Add(model);
            }
            else
            {
                var room = await _context.Rooms.FindAsync(model.RoomId);
                if (room == null) return NotFound();

                room.RoomName = model.RoomName;
                room.Price = model.Price;
                room.Capacity = model.Capacity;
                room.Status = model.Status;
                room.RoomType = model.RoomType;
                room.ImageUrl = model.ImageUrl;
                room.BranchId = model.BranchId;
                room.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<IActionResult> RoomsExport()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Branch)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("ID,Tên Phòng,Chi Nhánh,Loại Phòng,Giá,Trạng Thái");

            foreach (var r in rooms)
            {
                sb.AppendLine($"{r.RoomId},{r.RoomName},{r.Branch?.BranchName},{r.RoomType},{r.Price},{r.Status}");
            }

            return File(
                Encoding.UTF8.GetBytes(sb.ToString()),
                "text/csv",
                "rooms.csv");
        }
        // =========================
        // BOOKINGS
        // =========================
   
        public async Task<IActionResult> Bookings()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Authentications");

            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .OrderByDescending(b => b.BookingId)
                .ToListAsync();

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return NotFound();

            return Json(new
            {
                booking.BookingId,
                booking.Status,
                booking.CheckIn,
                booking.CheckOut,
                User = booking.User.FullName,
                Room = booking.Room.RoomName
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBookingStatus(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            booking.Status = status;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
