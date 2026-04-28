using BanVeXemPhim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Đăng nhập
        public IActionResult DangNhap(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Đăng nhập
        [HttpPost]
        public async Task<IActionResult> DangNhap(string email, string matKhau, string? returnUrl)
        {
            var kh = await _db.KhachHangs
                .FirstOrDefaultAsync(k => k.Email == email && k.MatKhau == matKhau);

            if (kh == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetString("UserId", kh.Id.ToString());
            HttpContext.Session.SetString("UserName", kh.HoTen);
            HttpContext.Session.SetString("UserRole", kh.VaiTro);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            if (kh.VaiTro == "Admin")
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Index", "Home");
        }

        // GET: Đăng ký
        public IActionResult DangKy()
        {
            return View();
        }

        // POST: Đăng ký
        [HttpPost]
        public async Task<IActionResult> DangKy(KhachHang kh, string xacNhanMatKhau)
        {
            if (kh.MatKhau != xacNhanMatKhau)
            {
                ViewBag.Error = "Mật khẩu và xác nhận mật khẩu không khớp!";
                return View(kh);
            }

            var exists = await _db.KhachHangs.AnyAsync(k => k.Email == kh.Email);
            if (exists)
            {
                ViewBag.Error = "Email này đã được đăng ký!";
                return View(kh);
            }

            kh.VaiTro = "User";
            kh.NgayTao = DateTime.Now;
            _db.KhachHangs.Add(kh);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        // Đăng xuất
        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Thông tin tài khoản
        public async Task<IActionResult> ThongTin()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (userIdStr == null)
                return RedirectToAction("DangNhap");

            int userId = int.Parse(userIdStr);
            var kh = await _db.KhachHangs.FindAsync(userId);
            return View(kh);
        }
    }
}
