using Booking_Hotel.Data;
using Booking_Hotel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking_Hotel.Controllers
{
    public class AuthenticationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthenticationsController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        // =========================
        // LOGIN
        // =========================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            ModelState.Remove("FullName");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
                return View(user);

            // 1️⃣ Tìm user theo email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser == null)
            {
                TempData["ErrorMessage"] = "Email không tồn tại!";
                return View(user);
            }

            // 2️⃣ Verify mật khẩu (CHUẨN)
            var result = _passwordHasher.VerifyHashedPassword(
                existingUser,
                existingUser.Password,
                user.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                TempData["ErrorMessage"] = "Mật khẩu không đúng!";
                return View(user);
            }

            // 3️⃣ LƯU SESSION
            HttpContext.Session.SetInt32("UserId", existingUser.UserId);
            HttpContext.Session.SetString("Username", existingUser.FullName);
            HttpContext.Session.SetString("Email", existingUser.Email);
            HttpContext.Session.SetString("UserRole", existingUser.Role);

            TempData["SuccessMessage"] = "Đăng nhập thành công!";

            // 4️⃣ PHÂN QUYỀN
            if (existingUser.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // REGISTER
        // =========================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // 1️⃣ Check email
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                TempData["ErrorMessage"] = "Email đã được sử dụng!";
                return View(user);
            }

            // 2️⃣ Set role
            user.Role = "User";

            // 3️⃣ HASH PASSWORD (CHUẨN)
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            // 4️⃣ Save
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
