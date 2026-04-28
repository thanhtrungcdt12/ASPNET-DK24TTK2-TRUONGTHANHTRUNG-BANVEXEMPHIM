using BanVeXemPhim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Controllers
{
    public class DatVeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DatVeController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Bước 1: Chọn ghế
        public async Task<IActionResult> ChonGhe(int suatChieuId)
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetString("UserId") == null)
                return RedirectToAction("DangNhap", "Account", new { returnUrl = $"/DatVe/ChonGhe/{suatChieuId}" });

            var suatChieu = await _db.SuatChieus
                .Include(s => s.Phim)
                .Include(s => s.PhongChieu)
                    .ThenInclude(pc => pc.RapChieu)
                .Include(s => s.PhongChieu)
                    .ThenInclude(pc => pc.Ghes)
                .FirstOrDefaultAsync(s => s.Id == suatChieuId);

            if (suatChieu == null) return NotFound();

            // Ghế đã được đặt trong suất này
            var gheDaDat = await _db.Ves
                .Where(v => v.SuatChieuId == suatChieuId && v.TrangThai == "DaDat")
                .Select(v => v.SoGhe)
                .ToListAsync();

            ViewBag.GheDaDat = gheDaDat;
            return View(suatChieu);
        }

        // Bước 2: Xác nhận đặt vé
        [HttpPost]
        public async Task<IActionResult> XacNhan(int suatChieuId, string soGheChon)
        {
            // Kiểm tra đăng nhập
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (userIdStr == null)
                return RedirectToAction("DangNhap", "Account");

            int userId = int.Parse(userIdStr);

            var suatChieu = await _db.SuatChieus
                .Include(s => s.Phim)
                .Include(s => s.PhongChieu)
                    .ThenInclude(pc => pc.RapChieu)
                .FirstOrDefaultAsync(s => s.Id == suatChieuId);

            if (suatChieu == null) return NotFound();

            // Tách danh sách ghế đã chọn (VD: "A1,A2,B3")
            var danhSachGhe = soGheChon.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            // Kiểm tra ghế có bị đặt rồi không
            var gheDaDat = await _db.Ves
                .Where(v => v.SuatChieuId == suatChieuId && v.TrangThai == "DaDat")
                .Select(v => v.SoGhe)
                .ToListAsync();

            var gheConflict = danhSachGhe.Intersect(gheDaDat).ToList();
            if (gheConflict.Any())
            {
                TempData["Error"] = $"Ghế {string.Join(", ", gheConflict)} đã được đặt. Vui lòng chọn ghế khác!";
                return RedirectToAction("ChonGhe", new { suatChieuId });
            }

            // Tạo vé cho từng ghế
            foreach (var ghe in danhSachGhe)
            {
                var ve = new Ve
                {
                    MaVe = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                    SuatChieuId = suatChieuId,
                    KhachHangId = userId,
                    SoGhe = ghe,
                    GiaTien = suatChieu.GiaVe,
                    NgayDat = DateTime.Now,
                    TrangThai = "DaDat"
                };
                _db.Ves.Add(ve);
            }
            await _db.SaveChangesAsync();

            TempData["Success"] = $"Đặt vé thành công! Bạn đã đặt {danhSachGhe.Count} vé.";
            return RedirectToAction("LichSu");
        }

        // Lịch sử đặt vé của người dùng
        public async Task<IActionResult> LichSu()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (userIdStr == null)
                return RedirectToAction("DangNhap", "Account");

            int userId = int.Parse(userIdStr);
            var ves = await _db.Ves
                .Include(v => v.SuatChieu)
                    .ThenInclude(s => s.Phim)
                .Include(v => v.SuatChieu)
                    .ThenInclude(s => s.PhongChieu)
                    .ThenInclude(pc => pc.RapChieu)
                .Where(v => v.KhachHangId == userId)
                .OrderByDescending(v => v.NgayDat)
                .ToListAsync();

            return View(ves);
        }

        // Hủy vé
        [HttpPost]
        public async Task<IActionResult> HuyVe(int id)
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (userIdStr == null)
                return RedirectToAction("DangNhap", "Account");

            int userId = int.Parse(userIdStr);
            var ve = await _db.Ves.FirstOrDefaultAsync(v => v.Id == id && v.KhachHangId == userId);
            if (ve != null)
            {
                ve.TrangThai = "DaHuy";
                await _db.SaveChangesAsync();
                TempData["Success"] = "Hủy vé thành công!";
            }
            return RedirectToAction("LichSu");
        }
    }
}
