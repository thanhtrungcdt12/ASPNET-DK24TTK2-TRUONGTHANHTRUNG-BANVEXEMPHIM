using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class Ve
    {
        public int Id { get; set; }

        [Display(Name = "Mã vé")]
        public string MaVe { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

        [Display(Name = "Số ghế")]
        public string SoGhe { get; set; } = "";

        [Display(Name = "Giá tiền")]
        public decimal GiaTien { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "DaDat"; // DaDat / DaHuy

        // Khóa ngoại
        public int SuatChieuId { get; set; }
        public SuatChieu? SuatChieu { get; set; }

        public int KhachHangId { get; set; }
        public KhachHang? KhachHang { get; set; }
    }
}
