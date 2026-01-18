using Booking_Hotel.Data;
using Booking_Hotel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking_Hotel.Controllers
{
    public class AuthenticationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // LOGIN
        // =========================

        // GET: /Authentications/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Authentications/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            // Chỉ validate Email & Password
            ModelState.Remove("FullName");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "Email không tồn tại!";
                return View(user);
            }

            if (existingUser.Password != user.Password)
            {
                TempData["ErrorMessage"] = "Mật khẩu không đúng!";
                return View(user);
            }

            // Lưu Session
            HttpContext.Session.SetInt32("UserId", existingUser.UserId);
            HttpContext.Session.SetString("UserEmail", existingUser.Email);
            HttpContext.Session.SetString("UserRole", existingUser.Role);
            HttpContext.Session.SetString("FullName", existingUser.FullName);
            TempData["SuccessMessage"] = "Đăng nhập thành công!";
            return RedirectToAction("Index", "Home");
        }

        // =========================
        // REGISTER
        // =========================

        // GET: /Authentications/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Authentications/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // 1. Kiểm tra email đã tồn tại chưa
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (emailExists)
            {
                TempData["ErrorMessage"] = "Email đã được sử dụng!";
                return View(user);
            }

            // 2. Set role mặc định
            user.Role = "User";

            // ⚠️ Demo: chưa hash mật khẩu (đồ án OK)
            // Nếu muốn nâng cao sẽ hash sau

            // 3. Lưu vào database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // =========================
        // FORGOT PASSWORD
        // =========================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // =========================
        // LOGOUT
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }
    }
}
