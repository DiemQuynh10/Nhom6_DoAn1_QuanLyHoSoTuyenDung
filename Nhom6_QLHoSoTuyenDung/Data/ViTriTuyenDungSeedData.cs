using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class ViTriTuyenDungSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.ViTriTuyenDungs.Any()) return;

            var now = new DateTime(2024, 1, 10);

            var danhSachViTri = new List<ViTriTuyenDung>
            {
                new ViTriTuyenDung
                {
                    MaViTri = "FE01",
                    TenViTri = "Frontend Developer",
                    SoLuongCanTuyen = 5,
                    TrangThai = "DangTuyen",
                    KyNang = "HTML, CSS, JavaScript, React",
                    PhongBanId = "PBIT",
                    NgayTao = now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "BE01",
                    TenViTri = "Backend Developer",
                    SoLuongCanTuyen = 4,
                    TrangThai = "DangTuyen",
                    KyNang = ".NET, C#, SQL Server",
                    PhongBanId = "PBIT",
                    NgayTao = now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "DA01",
                    TenViTri = "Data Analyst",
                    SoLuongCanTuyen = 3,
                    TrangThai = "DangTuyen",
                    KyNang = "Excel, SQL, Power BI, Python",
                    PhongBanId = "PBIT",
                    NgayTao = now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "BA01",
                    TenViTri = "Business Analyst",
                    SoLuongCanTuyen = 2,
                    TrangThai = "DangTuyen",
                    KyNang = "Phân tích nghiệp vụ, giao tiếp, viết tài liệu",
                    PhongBanId = "PBKD",
                    NgayTao = now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "KT01",
                    TenViTri = "Kế toán",
                    SoLuongCanTuyen = 2,
                    TrangThai = "DangTuyen",
                    KyNang = "Kế toán, Excel, nghiệp vụ tài chính",
                    PhongBanId = "PBKT",
                    NgayTao = now
                },
                new ViTriTuyenDung
                {
                    MaViTri = "QA01",
                    TenViTri = "Tester",
                    SoLuongCanTuyen = 3,
                    TrangThai = "DangTuyen",
                    KyNang = "Kiểm thử thủ công, automation test",
                    PhongBanId = "PBIT",
                    NgayTao = now
                },
            };

            context.ViTriTuyenDungs.AddRange(danhSachViTri);
            context.SaveChanges();
        }
    }
}
