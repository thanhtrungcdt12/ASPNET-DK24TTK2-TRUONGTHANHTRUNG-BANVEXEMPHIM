using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class KhachHang
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = ""; // Lưu dạng hash

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "User"; // User / Admin

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Navigation
        public ICollection<Ve> Ves { get; set; } = new List<Ve>();
    }
}
