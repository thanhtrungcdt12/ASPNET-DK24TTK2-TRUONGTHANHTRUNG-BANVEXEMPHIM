using BanVeXemPhim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Controllers
{
    public class PhimController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PhimController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Danh sách phim (có thể lọc theo thể loại)
        public async Task<IActionResult> Index(int? theLoaiId, string? tuKhoa)
        {
            var query = _db.Phims.Include(p => p.TheLoai).AsQueryable();

            if (theLoaiId.HasValue)
                query = query.Where(p => p.TheLoaiId == theLoaiId);

            if (!string.IsNullOrEmpty(tuKhoa))
                query = query.Where(p => p.TenPhim.Contains(tuKhoa));

            ViewBag.TheLoais = await _db.TheLoais.ToListAsync();
            ViewBag.TheLoaiId = theLoaiId;
            ViewBag.TuKhoa = tuKhoa;

            return View(await query.ToListAsync());
        }

        // Chi tiết phim + danh sách suất chiếu
        public async Task<IActionResult> ChiTiet(int id)
        {
            var phim = await _db.Phims
                .Include(p => p.TheLoai)
                .Include(p => p.SuatChieus)
                    .ThenInclude(s => s.PhongChieu)
                    .ThenInclude(pc => pc.RapChieu)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (phim == null) return NotFound();

            // Chỉ lấy suất chiếu từ hôm nay trở đi
            var suatChieus = phim.SuatChieus
                .Where(s => s.NgayChieu.Date >= DateTime.Today)
                .OrderBy(s => s.NgayChieu)
                .ThenBy(s => s.GioChieu)
                .ToList();

            ViewBag.SuatChieus = suatChieus;
            return View(phim);
        }
    }
}
