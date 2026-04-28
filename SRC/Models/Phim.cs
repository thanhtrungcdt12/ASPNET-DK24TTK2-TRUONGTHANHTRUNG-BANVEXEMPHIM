using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class Phim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phim")]
        [Display(Name = "Tên phim")]
        public string TenPhim { get; set; } = "";

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Đạo diễn")]
        public string? DaoDien { get; set; }

        [Display(Name = "Diễn viên")]
        public string? DienVien { get; set; }

        [Display(Name = "Thời lượng (phút)")]
        [Range(1, 500)]
        public int ThoiLuong { get; set; }

        [Display(Name = "Năm sản xuất")]
        public int NamSanXuat { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Trailer (YouTube URL)")]
        public string? TrailerUrl { get; set; }

        [Display(Name = "Ngày khởi chiếu")]
        [DataType(DataType.Date)]
        public DateTime NgayKhoiChieu { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "DangChieu"; // DangChieu / SapChieu / NgungChieu

        // Khóa ngoại
        public int TheLoaiId { get; set; }
        public TheLoai? TheLoai { get; set; }

        // Navigation
        public ICollection<SuatChieu> SuatChieus { get; set; } = new List<SuatChieu>();
    }
}
