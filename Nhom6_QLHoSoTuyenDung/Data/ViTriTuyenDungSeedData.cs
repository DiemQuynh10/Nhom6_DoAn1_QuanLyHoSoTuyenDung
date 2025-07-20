using Nhom6_QLHoSoTuyenDung.Models.Entities;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class ViTriTuyenDungSeedData
    {
            public static void Seed(AppDbContext context)
            {
                if (context.ViTriTuyenDungs.Any()) return;

                var danhSachViTri = new List<ViTriTuyenDung>
            {
                new ViTriTuyenDung
                {
                    MaViTri = "FE01",
                    TenViTri = "Frontend Developer",
                    SoLuongCanTuyen = 5,
                    TrangThai = "Đang tuyển",
                    KyNang = "HTML, CSS, JavaScript, React",
                    PhongBanId = "PBIT", // IT
                    NgayTao = DateTime.Now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "BE01",
                    TenViTri = "Backend Developer",
                    SoLuongCanTuyen = 4,
                    TrangThai = "Đang tuyển",
                    KyNang = ".NET, C#, SQL Server",
                    PhongBanId = "PBIT", // IT
                    NgayTao = DateTime.Now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "DA01",
                    TenViTri = "Data Analyst",
                    SoLuongCanTuyen = 3,
                    TrangThai = "Đang tuyển",
                    KyNang = "Excel, SQL, Power BI, Python",
                    PhongBanId = "PBIT", // IT
                    NgayTao = DateTime.Now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "BA01",
                    TenViTri = "Business Analyst",
                    SoLuongCanTuyen = 2,
                    TrangThai = "Đang tuyển",
                    KyNang = "Phân tích nghiệp vụ, giao tiếp, viết tài liệu",
                    PhongBanId = "PBKD", // Kinh doanh
                    NgayTao = DateTime.Now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "KT01",
                    TenViTri = "Kế toán",
                    SoLuongCanTuyen = 2,
                    TrangThai = "Đang tuyển",
                    KyNang = "Kế toán, Excel, nghiệp vụ tài chính",
                    PhongBanId = "PBKT", // kế toán
                    NgayTao = DateTime.Now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "QA01",
                    TenViTri = "Tester",
                    SoLuongCanTuyen = 3,
                    TrangThai = "Đang tuyển",
                    KyNang = "Kiểm thử thủ công, automation test",
                    PhongBanId = "PBIT", // IT
                    NgayTao = DateTime.Now
                },
            };

                context.ViTriTuyenDungs.AddRange(danhSachViTri);
                context.SaveChanges();
            }
    }
}
