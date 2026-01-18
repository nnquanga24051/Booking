using System.Diagnostics;
using Booking_Hotel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Hotel.Controllers
{
    public class AuthenticationsController : Controller
    {
        // GET: /Authentications/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Authentications/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User user)
        {
            // Chỉ validate Email và Password cho Login
            ModelState.Remove("FullName");
            ModelState.Remove("ConfirmPassword");

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // TODO: Xác thực với database
            // Ví dụ đơn giản:
            if (user.Email == "admin@hotel.com" && user.Password == "123456")
            {
                // Lưu thông tin đăng nhập vào Session hoặc Cookie
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", "Admin");

                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Email hoặc mật khẩu không đúng!";
                return View(user);
            }
        }

        // GET: /Authentications/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Authentications/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Kiểm tra mật khẩu xác nhận
            if (user.Password != user.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp!";
                return View(user);
            }

            // TODO: Kiểm tra email đã tồn tại chưa
            // TODO: Hash mật khẩu trước khi lưu
            // TODO: Lưu user vào database

            // Ví dụ đơn giản:
            user.Role = "User"; // Set default role
            // Lưu vào database ở đây

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        // GET: /Authentications/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }
    }
}