using Booking_Hotel.Data;
using Booking_Hotel.Models;
using Booking_Hotel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }

        //public async Task<IActionResult> Booking(
        //    int? branch,
        //    string? roomType,
        //    decimal? minPrice,
        //    decimal? maxPrice)
        //{
        //    var query = _context.Rooms
        //        .Include(r => r.Branch)
        //        .AsQueryable();

        //    // 🔹 Filter
        //    if (branch.HasValue)
        //        query = query.Where(r => r.BranchId == branch);

        //    if (!string.IsNullOrEmpty(roomType))
        //        query = query.Where(r => r.RoomType == roomType);

        //    if (minPrice.HasValue)
        //        query = query.Where(r => r.Price >= minPrice);

        //    if (maxPrice.HasValue)
        //        query = query.Where(r => r.Price <= maxPrice);

        //    var rooms = await query.Select(r => new BookingRoomVM
        //    {
        //        RoomId = r.RoomId,
        //        RoomName = r.RoomName,
        //        Price = r.Price,
        //        Status = r.Status,
        //        RoomType = r.RoomType,
        //        BranchName = r.Branch.BranchName
        //    }).ToListAsync();

        //    return View(rooms);
        //}

        public IActionResult Booking(
    int? branch,
    string? roomType,
    decimal? minPrice,
    decimal? maxPrice
)
        {
            ViewBag.Branches = _context.Branches.ToList();

            var rooms = _context.Rooms
                .Include(r => r.Branch)
                .Where(r => r.Status == "Available");

            if (branch.HasValue)
                rooms = rooms.Where(r => r.BranchId == branch);

            if (!string.IsNullOrEmpty(roomType))
                rooms = rooms.Where(r => r.RoomType == roomType);

            if (minPrice.HasValue)
                rooms = rooms.Where(r => r.Price >= minPrice);

            if (maxPrice.HasValue)
                rooms = rooms.Where(r => r.Price <= maxPrice);

            return View(rooms.ToList());
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
