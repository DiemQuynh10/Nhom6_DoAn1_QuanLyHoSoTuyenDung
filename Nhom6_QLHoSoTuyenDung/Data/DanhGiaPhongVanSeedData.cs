using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class DanhGiaPhongVanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.DanhGiaPhongVans.Any())
            {
                Console.WriteLine("⚠️ Đã có đánh giá phỏng vấn. Bỏ qua seed.");
                return;
            }

            var maNhanVien = context.NhanViens.Select(n => n.MaNhanVien).FirstOrDefault();
            if (string.IsNullOrEmpty(maNhanVien))
            {
                Console.WriteLine("❌ Không có nhân viên nào để gán đánh giá!");
                return;
            }

            var lichHoanThanh = context.LichPhongVans
                .Where(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString())
                .ToList();

            var danhSach = new List<DanhGiaPhongVan>();
            var rnd = new Random();
            int stt = 1;

            foreach (var lich in lichHoanThanh)
            {
                int gt = rnd.Next(6, 10);
                int cm = rnd.Next(6, 10);
                int vd = rnd.Next(6, 10);
                int td = rnd.Next(6, 10);
                int hh = rnd.Next(6, 10);

                float diemTB = (gt + cm + vd + td + hh) / 5f;

                DeXuatEnum dexuat;
                if (diemTB >= 8)
                    dexuat = DeXuatEnum.TiepNhan;
                else if (diemTB >= 6.5)
                    dexuat = rnd.NextDouble() > 0.5 ? DeXuatEnum.TiepNhan : DeXuatEnum.CanPhongVanLan2;
                else
                    dexuat = DeXuatEnum.TuChoi;

                danhSach.Add(new DanhGiaPhongVan
                {
                    Id = $"DG{stt++:0000}",
                    LichPhongVanId = lich.Id,
                    NhanVienDanhGiaId = maNhanVien, // ✅ THÊM DÒNG NÀY
                    GiaoTiep = gt,
                    KyNangChuyenMon = cm,
                    GiaiQuyetVanDe = vd,
                    ThaiDoLamViec = td,
                    TinhThanHocHoi = hh,
                    DiemDanhGia = diemTB,
                    NhanXet = $"Ứng viên có tiềm năng, {dexuat}",
                    DeXuat = dexuat.ToString()
                });
            }

            context.DanhGiaPhongVans.AddRange(danhSach);
            context.SaveChanges();

            Console.WriteLine($"✅ Đã seed {danhSach.Count} đánh giá phỏng vấn.");
        }
    }
}
