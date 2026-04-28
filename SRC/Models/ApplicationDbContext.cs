using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<TheLoai> TheLoais { get; set; }
        public DbSet<Phim> Phims { get; set; }
        public DbSet<RapChieu> RapChieus { get; set; }
        public DbSet<PhongChieu> PhongChieus { get; set; }
        public DbSet<Ghe> Ghes { get; set; }
        public DbSet<SuatChieu> SuatChieus { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<Ve> Ves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

        }
    }
}
