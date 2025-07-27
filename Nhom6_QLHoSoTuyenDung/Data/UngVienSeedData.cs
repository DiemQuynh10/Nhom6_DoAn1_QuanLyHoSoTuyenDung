using Newtonsoft.Json;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class UngVienSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.UngViens.Any()) return;

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "ungvien1000.json");
            Console.WriteLine("📄 Đang đọc file tại: " + jsonPath);

            if (!File.Exists(jsonPath))
            {
                Console.WriteLine("❌ Không tìm thấy file JSON!");
                return;
            }

            var json = File.ReadAllText(jsonPath);
            var danhSachUngVien = JsonConvert.DeserializeObject<List<UngVien>>(json)!;

            for (int i = 0; i < danhSachUngVien.Count; i++)
            {
                var ungVien = danhSachUngVien[i];

                // 👇 Gán Id mới nếu cần (nếu file json không có trường "Id" hoặc bị trùng)
                ungVien.MaUngVien = Guid.NewGuid().ToString();

                // Gán trạng thái theo cụm 200
                if (i < 200)
                    ungVien.TrangThai = TrangThaiUngVienEnum.Moi.ToString();
                else if (i < 400)
                    ungVien.TrangThai = TrangThaiUngVienEnum.DaPhongVan.ToString();
                else if (i < 550)
                    ungVien.TrangThai = TrangThaiUngVienEnum.DaCoLichVong2.ToString();
                else if (i < 700)
                    ungVien.TrangThai = TrangThaiUngVienEnum.TuChoi.ToString();
                else
                    ungVien.TrangThai = TrangThaiUngVienEnum.DaTuyen.ToString();
            }

            context.UngViens.AddRange(danhSachUngVien);
            context.SaveChanges();
            Console.WriteLine($"✅ Đã seed {danhSachUngVien.Count} ứng viên.");
        }
    }
}
