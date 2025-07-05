using Nhom6_QLHoSoTuyenDung.Models;
using System.Text.Json;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public class UngVienSeedData
    {
        public static void Seed(AppDbContext context)
        {
            // Nếu đã có dữ liệu thì không seed lại
            if (context.UngViens.Any()) return;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "ungvien1000.json");
            if (!File.Exists(filePath)) return;

            var json = File.ReadAllText(filePath);
            var danhSachUngVien = JsonSerializer.Deserialize<List<UngVien>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (danhSachUngVien != null && danhSachUngVien.Any())
            {
                context.UngViens.AddRange(danhSachUngVien);
                context.SaveChanges();
            }
        }
    }
}
