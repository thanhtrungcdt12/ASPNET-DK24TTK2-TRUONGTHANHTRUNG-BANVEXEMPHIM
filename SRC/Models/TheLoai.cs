using System.ComponentModel.DataAnnotations;

namespace BanVeXemPhim.Models
{
    public class TheLoai
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên thể loại")]
        [Display(Name = "Tên thể loại")]
        public string TenTheLoai { get; set; } = "";

        // Navigation property
        public ICollection<Phim> Phims { get; set; } = new List<Phim>();
    }
}
