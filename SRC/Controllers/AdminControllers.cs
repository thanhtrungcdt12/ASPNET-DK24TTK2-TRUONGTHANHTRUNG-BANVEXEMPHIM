using BanVeXemPhim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Controllers
{
    // Controller quản lý Dashboard Admin
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            ViewBag.TongPhim = await _db.Phims.CountAsync();
            ViewBag.TongVe = await _db.Ves.CountAsync();
            ViewBag.TongKhachHang = await _db.KhachHangs.CountAsync(k => k.VaiTro == "User");
            ViewBag.DoanhThu = await _db.Ves.Where(v => v.TrangThai == "DaDat").SumAsync(v => v.GiaTien);
            ViewBag.VeMoiNhat = await _db.Ves
                .Include(v => v.KhachHang)
                .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
                .OrderByDescending(v => v.NgayDat)
                .Take(5).ToListAsync();
            return View();
        }
    }

    // Controller quản lý Phim (Admin)
    public class QuanLyPhimController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyPhimController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var phims = await _db.Phims.Include(p => p.TheLoai).ToListAsync();
            return View(phims);
        }

        public async Task<IActionResult> Them()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            ViewBag.TheLoais = await _db.TheLoais.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Them(Phim phim)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            if (ModelState.IsValid)
            {
                _db.Phims.Add(phim);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm phim thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.TheLoais = await _db.TheLoais.ToListAsync();
            return View(phim);
        }

        public async Task<IActionResult> Sua(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var phim = await _db.Phims.FindAsync(id);
            if (phim == null) return NotFound();
            ViewBag.TheLoais = await _db.TheLoais.ToListAsync();
            return View(phim);
        }

        [HttpPost]
        public async Task<IActionResult> Sua(Phim phim)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            if (ModelState.IsValid)
            {
                _db.Phims.Update(phim);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật phim thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.TheLoais = await _db.TheLoais.ToListAsync();
            return View(phim);
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var phim = await _db.Phims.FindAsync(id);
            if (phim != null)
            {
                _db.Phims.Remove(phim);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa phim thành công!";
            }
            return RedirectToAction("Index");
        }
    }

    // Controller quản lý Suất Chiếu (Admin)
    public class QuanLySuatChieuController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLySuatChieuController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var suats = await _db.SuatChieus
                .Include(s => s.Phim)
                .Include(s => s.PhongChieu).ThenInclude(pc => pc.RapChieu)
                .OrderByDescending(s => s.NgayChieu)
                .ToListAsync();
            return View(suats);
        }

        public async Task<IActionResult> Them()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            ViewBag.Phims = await _db.Phims.ToListAsync();
            ViewBag.PhongChieus = await _db.PhongChieus.Include(p => p.RapChieu).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Them(SuatChieu suat)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            _db.SuatChieus.Add(suat);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thêm suất chiếu thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var suat = await _db.SuatChieus.FindAsync(id);
            if (suat != null)
            {
                _db.SuatChieus.Remove(suat);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa suất chiếu thành công!";
            }
            return RedirectToAction("Index");
        }
    }

    // Controller quản lý Vé (Admin)
    public class QuanLyVeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyVeController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var ves = await _db.Ves
                .Include(v => v.KhachHang)
                .Include(v => v.SuatChieu).ThenInclude(s => s.Phim)
                .OrderByDescending(v => v.NgayDat)
                .ToListAsync();
            return View(ves);
        }
    }

    // Controller quản lý Thể Loại (Admin)
    public class QuanLyTheLoaiController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyTheLoaiController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            return View(await _db.TheLoais.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Them(string tenTheLoai)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            if (!string.IsNullOrEmpty(tenTheLoai))
            {
                _db.TheLoais.Add(new TheLoai { TenTheLoai = tenTheLoai });
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm thể loại thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var tl = await _db.TheLoais.FindAsync(id);
            if (tl != null) { _db.TheLoais.Remove(tl); await _db.SaveChangesAsync(); }
            return RedirectToAction("Index");
        }
    }
}
