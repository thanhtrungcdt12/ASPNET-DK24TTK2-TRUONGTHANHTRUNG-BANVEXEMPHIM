using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class SuatChieu
    {
        public int Id { get; set; }

        [Display(Name = "Ngày chiếu")]
        [DataType(DataType.Date)]
        public DateTime NgayChieu { get; set; }

        [Display(Name = "Giờ chiếu")]
        [DataType(DataType.Time)]
        public TimeSpan GioChieu { get; set; }

        [Display(Name = "Giá vé (VNĐ)")]
        [Range(0, 1000000)]
        public decimal GiaVe { get; set; }

        // Khóa ngoại
        public int PhimId { get; set; }
        public Phim? Phim { get; set; }

        public int PhongChieuId { get; set; }
        public PhongChieu? PhongChieu { get; set; }

        // Navigation
        public ICollection<Ve> Ves { get; set; } = new List<Ve>();
    }
}
