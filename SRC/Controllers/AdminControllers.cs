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

    // Controller quản lý Rạp Chiếu (Admin)
    public class QuanLyRapController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyRapController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var raps = await _db.RapChieus.Include(r => r.PhongChieus).ToListAsync();
            return View(raps);
        }

        [HttpPost]
        public async Task<IActionResult> Them(RapChieu rap)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            if (ModelState.IsValid)
            {
                _db.RapChieus.Add(rap);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Thêm rạp thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Sua(RapChieu rap)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            if (ModelState.IsValid)
            {
                _db.RapChieus.Update(rap);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Cập nhật rạp thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var rap = await _db.RapChieus.FindAsync(id);
            if (rap != null)
            {
                _db.RapChieus.Remove(rap);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa rạp thành công!";
            }
            return RedirectToAction("Index");
        }
    }

    // Controller quản lý Phòng Chiếu (Admin)
    public class QuanLyPhongController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyPhongController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            ViewBag.RapChieus = await _db.RapChieus.ToListAsync();
            var phongs = await _db.PhongChieus
                .Include(p => p.RapChieu)
                .OrderBy(p => p.RapChieuId).ThenBy(p => p.TenPhong)
                .ToListAsync();
            return View(phongs);
        }

        [HttpPost]
        public async Task<IActionResult> Them(PhongChieu phong)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            _db.PhongChieus.Add(phong);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thêm phòng thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Sua(PhongChieu phong)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            _db.PhongChieus.Update(phong);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật phòng thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var phong = await _db.PhongChieus.FindAsync(id);
            if (phong != null)
            {
                _db.PhongChieus.Remove(phong);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa phòng thành công!";
            }
            return RedirectToAction("Index");
        }
    }

    // Controller quản lý Ghế (Admin)
    public class QuanLyGheController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyGheController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index(int? phongChieuId)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            ViewBag.PhongChieus = await _db.PhongChieus.Include(p => p.RapChieu).ToListAsync();
            ViewBag.PhongChieuId = phongChieuId;

            var query = _db.Ghes.Include(g => g.PhongChieu).ThenInclude(p => p!.RapChieu).AsQueryable();
            if (phongChieuId.HasValue)
                query = query.Where(g => g.PhongChieuId == phongChieuId.Value);

            var ghes = await query.OrderBy(g => g.PhongChieuId).ThenBy(g => g.SoGhe).ToListAsync();
            return View(ghes);
        }

        [HttpPost]
        public async Task<IActionResult> Them(Ghe ghe)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            _db.Ghes.Add(ghe);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thêm ghế thành công!";
            return RedirectToAction("Index", new { phongChieuId = ghe.PhongChieuId });
        }

        [HttpPost]
        public async Task<IActionResult> Sua(Ghe ghe)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            _db.Ghes.Update(ghe);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật ghế thành công!";
            return RedirectToAction("Index", new { phongChieuId = ghe.PhongChieuId });
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var ghe = await _db.Ghes.FindAsync(id);
            int? phongId = ghe?.PhongChieuId;
            if (ghe != null)
            {
                _db.Ghes.Remove(ghe);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa ghế thành công!";
            }
            return RedirectToAction("Index", new { phongChieuId = phongId });
        }
    }

    // Controller quản lý Tài Khoản (Admin)
    public class QuanLyTaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _db;
        public QuanLyTaiKhoanController(ApplicationDbContext db) { _db = db; }

        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var khachHangs = await _db.KhachHangs.OrderBy(k => k.Id).ToListAsync();
            return View(khachHangs);
        }

        [HttpPost]
        public async Task<IActionResult> Them(KhachHang kh)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            if (await _db.KhachHangs.AnyAsync(k => k.Email == kh.Email))
            {
                TempData["Error"] = "Email này đã được đăng ký!";
                return RedirectToAction("Index");
            }
            kh.NgayTao = DateTime.Now;
            _db.KhachHangs.Add(kh);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Thêm tài khoản thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Sua(int id, string hoTen, string email, string? matKhau, string? soDienThoai, string vaiTro)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");
            var kh = await _db.KhachHangs.FindAsync(id);
            if (kh == null) return RedirectToAction("Index");

            kh.HoTen = hoTen;
            kh.Email = email;
            kh.SoDienThoai = soDienThoai;
            kh.VaiTro = vaiTro;
            // Chỉ đổi mật khẩu khi admin nhập mật khẩu mới
            if (!string.IsNullOrWhiteSpace(matKhau)) kh.MatKhau = matKhau;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Cập nhật tài khoản thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            if (!IsAdmin()) return RedirectToAction("DangNhap", "Account");

            // Không cho admin xóa chính tài khoản đang đăng nhập
            if (HttpContext.Session.GetString("UserId") == id.ToString())
            {
                TempData["Error"] = "Không thể xóa tài khoản đang đăng nhập!";
                return RedirectToAction("Index");
            }

            var kh = await _db.KhachHangs.FindAsync(id);
            if (kh != null)
            {
                _db.KhachHangs.Remove(kh);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Xóa tài khoản thành công!";
            }
            return RedirectToAction("Index");
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
            return View(await _db.TheLoais.Include(t => t.Phims).ToListAsync());
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
