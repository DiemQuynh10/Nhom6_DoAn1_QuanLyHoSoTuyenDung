using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class CapNhatTrangThaiUngVienSauDanhGia
    {
        public static void CapNhat(AppDbContext context)
        {
            var lichHoanThanh = context.LichPhongVans
                .Where(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString())
                .ToList();

            int capNhat = 0;

            foreach (var lich in lichHoanThanh)
            {
                var ungVien = context.UngViens.FirstOrDefault(u => u.MaUngVien == lich.UngVienId);
                var danhGia = context.DanhGiaPhongVans.FirstOrDefault(d => d.LichPhongVanId == lich.Id);

                if (ungVien == null || danhGia == null)
                    continue;

                var deXuat = danhGia.DeXuat;

                if (deXuat == DeXuatEnum.TiepNhan.ToString())
                {
                    ungVien.TrangThai = TrangThaiUngVienEnum.DaTuyen.ToString();
                }
                else if (deXuat == DeXuatEnum.CanPhongVanLan2.ToString())
                {
                    // Kiểm tra nếu đã có lịch vòng 2 thì chuyển sang DaCoLichVong2
                    bool coLichV2 = context.LichPhongVans.Any(l =>
                        l.UngVienId == ungVien.MaUngVien &&
                        l.Id != lich.Id &&
                        l.ThoiGian > lich.ThoiGian &&
                        l.TrangThai == TrangThaiPhongVanEnum.DaLenLich.ToString());

                    ungVien.TrangThai = coLichV2
                        ? TrangThaiUngVienEnum.DaCoLichVong2.ToString()
                        : TrangThaiUngVienEnum.CanPhongVanLan2.ToString();
                }
                else if (deXuat == DeXuatEnum.TuChoi.ToString())
                {
                    ungVien.TrangThai = TrangThaiUngVienEnum.TuChoi.ToString();
                }

                capNhat++;
            }

            context.SaveChanges();
            Console.WriteLine($"✅ Đã cập nhật trạng thái cho {capNhat} ứng viên.");
        }
    }
}
