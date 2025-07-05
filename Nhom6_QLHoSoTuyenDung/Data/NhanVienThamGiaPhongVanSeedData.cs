using Nhom6_QLHoSoTuyenDung.Models;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class NhanVienThamGiaPhongVanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.NhanVienThamGiaPhongVans.Any()) return;

            var dsThamGia = new List<NhanVienThamGiaPhongVan>();
            var rnd = new Random();
            int stt = 1;

            for (int i = 1; i <= 40; i++)
            {
                var lichId = $"LPV{i:000}";

                // Gán vai trò chủ trì theo kiểu: nếu là backend/frontend thì do chuyên môn, còn lại HR
                string viTri = context.LichPhongVans.First(l => l.Id == lichId).ViTriId;

                string chuTriNV = (viTri == "BE01" || viTri == "FE01")
                    ? "NV003" // kỹ thuật làm chủ trì
                    : "NV002"; // HR làm chủ trì

                dsThamGia.Add(new NhanVienThamGiaPhongVan
                {
                    Id = $"TGPV{stt++:000}",
                    LichPhongVanId = lichId,
                    NhanVienId = chuTriNV,
                    VaiTro = "Chủ trì"
                });

                // HR hỗ trợ ghi chép
                dsThamGia.Add(new NhanVienThamGiaPhongVan
                {
                    Id = $"TGPV{stt++:000}",
                    LichPhongVanId = lichId,
                    NhanVienId = "NV010", // HR khác
                    VaiTro = "Ghi chép"
                });

                // Phỏng vấn viên chuyên môn
                dsThamGia.Add(new NhanVienThamGiaPhongVan
                {
                    Id = $"TGPV{stt++:000}",
                    LichPhongVanId = lichId,
                    NhanVienId = "NV004", // chuyên môn khác
                    VaiTro = "Đánh giá"
                });

                // Có 50% lịch sẽ có thêm lãnh đạo tham gia
                if (rnd.NextDouble() > 0.5)
                {
                    dsThamGia.Add(new NhanVienThamGiaPhongVan
                    {
                        Id = $"TGPV{stt++:000}",
                        LichPhongVanId = lichId,
                        NhanVienId = "NV001",
                        VaiTro = "Giám sát"
                    });
                }
            }

            context.NhanVienThamGiaPhongVans.AddRange(dsThamGia);
            context.SaveChanges();
        }
    }
}
