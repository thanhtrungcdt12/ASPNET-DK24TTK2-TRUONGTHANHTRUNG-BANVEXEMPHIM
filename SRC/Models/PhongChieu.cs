using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class PhongChieu
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tên phòng")]
        public string TenPhong { get; set; } = "";

        [Display(Name = "Số ghế")]
        public int SoGhe { get; set; }

        // Khóa ngoại
        public int RapChieuId { get; set; }
        public RapChieu? RapChieu { get; set; }

        // Navigation
        public ICollection<Ghe> Ghes { get; set; } = new List<Ghe>();
        public ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
    }
}
