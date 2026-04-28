using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class Ghe
    {
        public int Id { get; set; }

        [Display(Name = "Số ghế")]
        public string SoGhe { get; set; } = ""; // VD: A1, A2, B1...

        [Display(Name = "Loại ghế")]
        public string LoaiGhe { get; set; } = "Thuong"; // Thuong / VIP

        // Khóa ngoại
        public int PhongChieuId { get; set; }
        public PhongChieu? PhongChieu { get; set; }
    }
}
