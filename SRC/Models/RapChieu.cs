using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class RapChieu
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên rạp")]
        public string TenRap { get; set; } = "";

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        // Navigation
        public ICollection<PhongChieu> PhongChieus { get; set; } = new List<PhongChieu>();
    }
}
