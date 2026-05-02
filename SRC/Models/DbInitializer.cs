using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BanVeXemPhim.Models
{
    public static class DbInitializer
    {
        private const string DbName = "TruongThanhTrungTiket";
        private const string MasterConnectionString =
            "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static void Initialize(string contentRootPath, ApplicationDbContext db)
        {
            var bakPath = Path.Combine(contentRootPath, $"{DbName}.bak");

            if (File.Exists(bakPath))
            {
                RestoreFromBackup(bakPath);
                return;
            }

            // Không có file .bak: tạo database code-first và seed dữ liệu mẫu lần đầu chạy
            if (db.Database.EnsureCreated())
                SeedSampleData(db);
        }

        private static void RestoreFromBackup(string bakPath)
        {
            using var conn = new SqlConnection(MasterConnectionString);
            conn.Open();

            if (DatabaseExists(conn)) return;

            var (dataLogical, logLogical) = GetLogicalFileNames(conn, bakPath);
            var dataDir = Path.GetDirectoryName(bakPath)!;
            var mdfPath = Path.Combine(dataDir, $"{DbName}.mdf");
            var ldfPath = Path.Combine(dataDir, $"{DbName}_log.ldf");

            using var cmd = conn.CreateCommand();
            cmd.CommandTimeout = 120;
            cmd.CommandText = $@"
                RESTORE DATABASE [{DbName}]
                FROM DISK = N'{bakPath}'
                WITH MOVE N'{dataLogical}' TO N'{mdfPath}',
                     MOVE N'{logLogical}' TO N'{ldfPath}',
                     REPLACE";
            cmd.ExecuteNonQuery();
        }

        private static bool DatabaseExists(SqlConnection conn)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT DB_ID(N'{DbName}')";
            var result = cmd.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }

        private static (string DataLogical, string LogLogical) GetLogicalFileNames(SqlConnection conn, string bakPath)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"RESTORE FILELISTONLY FROM DISK = N'{bakPath}'";
            using var reader = cmd.ExecuteReader();

            string dataLogical = "", logLogical = "";
            while (reader.Read())
            {
                var type = reader["Type"].ToString();
                var logical = reader["LogicalName"]?.ToString() ?? "";
                if (type == "D") dataLogical = logical;
                else if (type == "L") logLogical = logical;
            }
            return (dataLogical, logLogical);
        }

        private static void SeedSampleData(ApplicationDbContext db)
        {
            // Thể loại
            var hanhDong = new TheLoai { TenTheLoai = "Hành động" };
            var haiHuoc = new TheLoai { TenTheLoai = "Hài hước" };
            var kinhDi = new TheLoai { TenTheLoai = "Kinh dị" };
            var tinhCam = new TheLoai { TenTheLoai = "Tình cảm" };
            var hoatHinh = new TheLoai { TenTheLoai = "Hoạt hình" };
            var phieuLuu = new TheLoai { TenTheLoai = "Phiêu lưu" };
            db.TheLoais.AddRange(hanhDong, haiHuoc, kinhDi, tinhCam, hoatHinh, phieuLuu);

            // Rạp chiếu
            var rap1 = new RapChieu { TenRap = "Rạp Chiếu Phim Trương Thành Trung", DiaChi = "123 Hùng Vương, Q5, TP.HCM", SoDienThoai = "028 1234 5678" };
            var rap2 = new RapChieu { TenRap = "Lotte Cinema Sư Vạn Hạnh", DiaChi = "11 Sư Vạn Hạnh, Q10, TP.HCM", SoDienThoai = "028 8765 4321" };
            db.RapChieus.AddRange(rap1, rap2);

            // Phòng chiếu
            var phong1 = new PhongChieu { TenPhong = "Phòng 1", SoGhe = 50, RapChieu = rap1 };
            var phong2 = new PhongChieu { TenPhong = "Phòng 2", SoGhe = 60, RapChieu = rap1 };
            var phong3 = new PhongChieu { TenPhong = "Phòng 1", SoGhe = 50, RapChieu = rap2 };
            db.PhongChieus.AddRange(phong1, phong2, phong3);

            // Phim
            var phim1 = new Phim
            {
                TenPhim = "Heo Năm Móng",
                TheLoai = kinhDi,
                MoTa = "Truyền thuyết Cô Năm Hợi được truyền miệng qua nhiều thế hệ, như một lời nhắc về sự tái sinh đầy nghiệt ngã: linh hồn chết oan hoặc mang nhiều nghiệp quả nên mắc kẹt trong thân xác loài vật, mang theo ký ức và oán lệnh chưa thể hóa giải.",
                DaoDien = "Lưu Thành Luân",
                DienVien = "Võ Tấn Phát, Trần Ngọc Vàng, Ốc Thanh Vân",
                ThoiLuong = 103,
                NamSanXuat = 2026,
                NgayKhoiChieu = new DateTime(2026, 4, 20),
                TrangThai = "DangChieu",
                HinhAnh = "https://cdn.galaxycine.vn/media/2026/4/20/heo-nam-mong-2_1776693195637.jpg"
            };
            var phim2 = new Phim
            {
                TenPhim = "Trùm Sò",
                TheLoai = haiHuoc,
                MoTa = "Ở Làng Sứa Đỏ - một ngôi làng nhỏ xa xôi heo hút, hạn hán triền miên, người dân ai cũng nghèo cũng khổ, chỉ riêng Trùm Sò là giàu nứt đố đổ vách. Ghét nỗi là gã sống không tình không nghĩa, chỉ biết đến tiền, tiền và tiền. Gã còn ki bo, bủn xỉn với cả chính bản thân mình: ăn không dám ăn, mặc không dám mặc, chẳng chơi chẳng yêu cũng chẳng chịu cưới ai. Ngoài bà mẹ già lú lẫn thì chớ hòng ai bòn được của gã một cắc nào.",
                DaoDien = "Đỗ Đức Thịnh",
                DienVien = "Đức Thịnh, Phương Nam, Mai Phương, Doãn Quốc Đam",
                ThoiLuong = 105,
                NamSanXuat = 2026,
                NgayKhoiChieu = new DateTime(2026, 4, 20),
                TrangThai = "DangChieu",
                HinhAnh = "https://cdn.galaxycine.vn/media/2026/4/20/500_1776651063174.jpg"
            };
            var phim3 = new Phim
            {
                TenPhim = "Cá Con Cau Có",
                TheLoai = hoatHinh,
                MoTa = "Giữa lòng đại dương nhộn nhịp, chú cá cô độc Mr. Fish sống tách biệt khỏi thế giới cho đến khi cuộc đời anh bị đảo lộn bởi Pip - một “cô rồng biển” nhỏ hiếu động vô tình gây ra thảm họa khiến cả hai cùng mất nhà. Bị cuốn vào hành trình sửa sai, họ quyết định tìm đến Shimmer - sinh vật huyền thoại được cho là có thể ban điều ước, trong khi đồng thời phải chạy đua với Benji, một chú mực trẻ đang tuyệt vọng cứu lấy cộng đồng của mình. Nhưng khi sự thật được hé lộ rằng Shimmer không thể thực hiện điều ước, cả ba buộc phải đối mặt với chính mình, học cách tin tưởng và hợp lực, để nhận ra rằng phép màu thực sự luôn nằm trong chính họ.",
                DaoDien = "Ricard Cussó, Rio Harrington",
                DienVien = "Đang cập nhật",
                ThoiLuong = 92,
                NamSanXuat = 2026,
                NgayKhoiChieu = new DateTime(2026, 4, 26),
                TrangThai = "DangChieu",
                HinhAnh = "https://cdn.galaxycine.vn/media/2026/4/6/con-ca-cau-co-500_1775458864686.jpg"
            };
            var phim4 = new Phim
            {
                TenPhim = "Phim Super Mario Thiên Hà",
                TheLoai = hoatHinh,
                MoTa = "Phim Super Mario Thiên Hà là một bộ phim hoạt hình được lấy bối cảnh trong thế giới của Anh Em Super Mario và là phần tiếp theo của Phim Anh Em Super Mario – tác phẩm ra mắt năm 2023 và đạt doanh thu hơn 1,3 tỷ đô la trên toàn cầu. Cả hai bộ phim Phim Anh Em Super Mario (2023) và Phim Super Mario Thiên Hà đều do Chris Meledandri (hãng Illumination) và Shigeru Miyamoto (từ Nintendo) đồng sản xuất.",
                DaoDien = "Aaron Horvath, Michael Jelenic",
                DienVien = "Chris Pratt, Anya Taylor-Joy, Jack Black",
                ThoiLuong = 99,
                NamSanXuat = 2026,
                NgayKhoiChieu = new DateTime(2026, 4, 1),
                TrangThai = "DangChieu",
                HinhAnh = "https://cdn.galaxycine.vn/media/2026/4/1/mario-500_1775018072523.jpg"
            };
            var phim5 = new Phim
            {
                TenPhim = "Minions & Quái Vật",
                TheLoai = phieuLuu,
                MoTa = "Minions & Quái Vật là câu chuyện vừa náo loạn, vừa ngớ ngẩn nhưng “hoàn toàn có thật” về cách Minions chinh phục Hollywood, trở thành ngôi sao điện ảnh, rồi mất tất cả, vô tình thả quái vật ra khắp thế giới và sau đó phải cùng nhau hợp sức để cứu lấy hành tinh khỏi chính mớ hỗn loạn mà mình tạo ra.\r\n\r\nXem thêm tại: https://www.galaxycine.vn/dat-ve/minions--monsters/",
                DaoDien = "Pierre Coffin",
                DienVien = "Pierre Coffin",
                ThoiLuong = 100,
                NamSanXuat = 2026,
                NgayKhoiChieu = new DateTime(2026, 7, 1),
                TrangThai = "SapChieu",
                HinhAnh = "https://cdn.galaxycine.vn/media/2026/2/11/minions--monsters-500_1770783510822.jpg"
            };
            var phim6 = new Phim
            {
                TenPhim = "Phi Công Siêu Đẳng Maverick",
                TheLoai = hanhDong,
                MoTa = "Pete “Maverick” Mitchell từng nổi danh là một phi công thử nghiệm quả cảm hàng đầu của Hải quân. Sau hơn ba mươi năm phục vụ, anh né tránh cơ hội thăng chức khiến bản thân cảm thấy bị bó buộc, để trở về làm chính mình - một đại úy. Trong đợt đào tạo biệt đội tại trường quân sự Top Gun cho nhiệm vụ chuyên biệt chưa từng có, Maverick chạm trán với Trung úy Bradley Bradshaw (Miles Teller) - con trai của người bạn thân quá cố Nick Bradshaw.",
                DaoDien = "Joseph Kosinski",
                DienVien = "Tom Cruise, Miles Teller, Val Kilmer, Jon Hamm",
                ThoiLuong = 130,
                NamSanXuat = 2026,
                NgayKhoiChieu = new DateTime(2026, 5, 13),
                TrangThai = "SapChieu",
                HinhAnh = "https://cdn.galaxycine.vn/media/2022/5/27/1200wx1800h_1653624077615.jpg"
            };
            db.Phims.AddRange(phim1, phim2, phim3, phim4, phim5, phim6);

            // Suất chiếu
            db.SuatChieus.AddRange(
                new SuatChieu { Phim = phim1, PhongChieu = phong1, NgayChieu = new DateTime(2026, 4, 30), GioChieu = new TimeSpan(9, 0, 0), GiaVe = 90000 },
                new SuatChieu { Phim = phim1, PhongChieu = phong1, NgayChieu = new DateTime(2026, 5, 1), GioChieu = new TimeSpan(13, 30, 0), GiaVe = 100000 },
                new SuatChieu { Phim = phim2, PhongChieu = phong2, NgayChieu = new DateTime(2026, 5, 3), GioChieu = new TimeSpan(10, 0, 0), GiaVe = 85000 },
                new SuatChieu { Phim = phim3, PhongChieu = phong1, NgayChieu = new DateTime(2026, 5, 7), GioChieu = new TimeSpan(19, 0, 0), GiaVe = 95000 }
            );

            // Ghế cho phòng 1 (5 hàng x 10 ghế = 50 ghế)
            string[] hang = { "A", "B", "C", "D", "E" };
            foreach (var h in hang)
            {
                for (int i = 1; i <= 10; i++)
                {
                    db.Ghes.Add(new Ghe
                    {
                        PhongChieu = phong1,
                        SoGhe = $"{h}{i}",
                        LoaiGhe = (h == "D" || h == "E") ? "VIP" : "Thuong"
                    });
                }
            }

            foreach (var h in hang)
            {
                for (int i = 1; i <= 10; i++)
                {
                    db.Ghes.Add(new Ghe
                    {
                        PhongChieu = phong2,
                        SoGhe = $"{h}{i}",
                        LoaiGhe = (h == "D" || h == "E") ? "VIP" : "Thuong"
                    });
                }
            }

            foreach (var h in hang)
            {
                for (int i = 1; i <= 10; i++)
                {
                    db.Ghes.Add(new Ghe
                    {
                        PhongChieu = phong3,
                        SoGhe = $"{h}{i}",
                        LoaiGhe = (h == "D" || h == "E") ? "VIP" : "Thuong"
                    });
                }
            }

            // Tài khoản
            db.KhachHangs.AddRange(
                new KhachHang
                {
                    HoTen = "Admin",
                    Email = "admin@cinema.com",
                    MatKhau = "admin123",
                    VaiTro = "Admin",
                    NgayTao = new DateTime(2026, 4, 1)
                },
                new KhachHang
                {
                    HoTen = "Trương Thành Trung",
                    Email = "thanhtrungcdt12@gmail.com",
                    MatKhau = "user123",
                    VaiTro = "User",
                    NgayTao = new DateTime(2026, 4, 1)
                }
            );

            db.SaveChanges();
        }
    }
}
