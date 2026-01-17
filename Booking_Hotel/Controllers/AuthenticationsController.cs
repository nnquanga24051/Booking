using System.Diagnostics;
using Booking_Hotel.Models;
using Microsoft.AspNetCore.Mvc;

namespace Booking_Hotel.Controllers
{
    public class AuthenticationsController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }

}
