using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class PhongBanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.PhongBans.Any()) return;

            var phongBans = new List<PhongBan>
            {
                new PhongBan { Id = "PBKT", TenPhong = "Phòng Kế toán", MoTa = "Phụ trách kế toán, tài chính." },
                new PhongBan { Id = "PBNS", TenPhong = "Phòng Nhân sự", MoTa = "Phụ trách tuyển dụng, đào tạo." },
                new PhongBan { Id = "PBIT", TenPhong = "Phòng IT", MoTa = "Phụ trách phát triển phần mềm, hệ thống." },
                new PhongBan { Id = "PBKD", TenPhong = "Phòng Kinh doanh", MoTa = "Phụ trách bán hàng, phát triển thị trường." },
                new PhongBan { Id = "PBDA", TenPhong = "Phòng Dữ liệu", MoTa = "Phụ trách phân tích và khai thác dữ liệu." },
            };

            context.PhongBans.AddRange(phongBans);
            context.SaveChanges();
        }
    }
}
