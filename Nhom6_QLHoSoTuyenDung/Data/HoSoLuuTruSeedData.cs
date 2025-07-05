using Nhom6_QLHoSoTuyenDung.Models;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class HoSoLuuTruSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.HoSoLuuTrus.Any()) return;

            var dsLuuTru = new List<HoSoLuuTru>();
            var rnd = new Random();
            int stt = 1;

            // Lấy danh sách ứng viên có trạng thái "Trượt"
            var ungVienTruot = context.UngViens
                .Where(uv => uv.TrangThai.ToLower().Contains("trượt"))
                .Take(30)
                .ToList();

            foreach (var uv in ungVienTruot)
            {
                dsLuuTru.Add(new HoSoLuuTru
                {
                    Id = $"HSLT{stt++:000}",
                    UngVienId = uv.MaUngVien,
                    ViTriId = uv.ViTriUngTuyenId,
                    LyDoLuuTru = "Hồ sơ tiềm năng, không phù hợp thời điểm này",
                    NgayLuu = DateTime.Now.AddDays(-rnd.Next(1, 60))
                });
            }

            context.HoSoLuuTrus.AddRange(dsLuuTru);
            context.SaveChanges();
        }
    }
}
