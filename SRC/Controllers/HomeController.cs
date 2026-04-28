using BanVeXemPhim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Trang chủ - hiển thị phim đang chiếu và sắp chiếu
        public async Task<IActionResult> Index()
        {
            var phimDangChieu = await _db.Phims
                .Include(p => p.TheLoai)
                .Where(p => p.TrangThai == "DangChieu")
                .ToListAsync();

            var phimSapChieu = await _db.Phims
                .Include(p => p.TheLoai)
                .Where(p => p.TrangThai == "SapChieu")
                .ToListAsync();

            ViewBag.PhimDangChieu = phimDangChieu;
            ViewBag.PhimSapChieu = phimSapChieu;
            return View();
        }

        public IActionResult GioiThieu()
        {
            return View();
        }
    }
}
