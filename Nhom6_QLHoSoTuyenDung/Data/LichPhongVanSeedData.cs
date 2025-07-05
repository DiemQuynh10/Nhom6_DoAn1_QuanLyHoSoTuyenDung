using Nhom6_QLHoSoTuyenDung.Models;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class LichPhongVanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.LichPhongVans.Any()) return;

            var lichList = new List<LichPhongVan>();
            var viTriIds = new[] { "FE01", "BE01", "KT01", "DA01", "BA01" };
            var phongIds = new[] { "PPV001", "PPV002", "PPV003", "PPV004", "PPV005" };
            var trangThaiList = new[] { "Đã lên lịch", "Hoàn thành", "Hủy" };
            var rnd = new Random();

            for (int i = 1; i <= 40; i++)
            {
                var lich = new LichPhongVan
                {
                    Id = $"LPV{i:000}",
                    PhongPhongVanId = phongIds[rnd.Next(phongIds.Length)],
                    UngVienId = $"UV{i:0000}",
                    ViTriId = viTriIds[rnd.Next(viTriIds.Length)],
                    ThoiGian = DateTime.Now.AddDays(-rnd.Next(1, 30)).AddHours(rnd.Next(8, 17)),
                    TrangThai = trangThaiList[rnd.Next(trangThaiList.Length)],
                    GhiChu = "Ứng viên cần mang theo CV giấy"
                };
                lichList.Add(lich);
            }

            context.LichPhongVans.AddRange(lichList);
            context.SaveChanges();
        }
    }
}
