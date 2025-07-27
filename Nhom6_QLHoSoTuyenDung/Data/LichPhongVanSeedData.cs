using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;

namespace Nhom6_QLHoSoTuyenDung.Data
{
    public static class LichPhongVanSeedData
    {
        public static void Seed(AppDbContext context)
        {
            if (context.LichPhongVans.Any())
            {
                Console.WriteLine("⚠️ Bảng LichPhongVans đã có dữ liệu. Bỏ qua seed.");
                return;
            }

            var rnd = new Random();
            var ungViens = context.UngViens.ToList();
            var phongIds = context.PhongPhongVans.Select(p => p.Id).ToList();
            var viTriIds = context.ViTriTuyenDungs.Select(v => v.MaViTri).ToList();
            var interviewers = context.NguoiDungs
                .Where(x => x.VaiTro == "Interviewer")
                .Select(x => x.NhanVienId)
                .ToList();

            if (!ungViens.Any() || !phongIds.Any() || !viTriIds.Any() || !interviewers.Any())
            {
                Console.WriteLine("❌ Thiếu dữ liệu liên quan đến ứng viên, phòng, vị trí hoặc người phỏng vấn.");
                return;
            }

            var lichList = new List<LichPhongVan>();
            var thamGiaList = new List<NhanVienThamGiaPhongVan>();
            int stt = 1;

            foreach (var uv in ungViens)
            {
                if (uv.TrangThai == TrangThaiUngVienEnum.Moi.ToString())
                    continue;

                var isVong2 = uv.TrangThai == TrangThaiUngVienEnum.DaCoLichVong2.ToString();
                var isVong1 = uv.TrangThai == TrangThaiUngVienEnum.CanPhongVanLan2.ToString()
                            || uv.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString()
                            || uv.TrangThai == TrangThaiUngVienEnum.TuChoi.ToString()
                            || uv.TrangThai == TrangThaiUngVienEnum.DaPhongVan.ToString();

                if (!isVong1 && !isVong2) continue;

                var lich = new LichPhongVan
                {
                    Id = $"LPV{stt:0000}",
                    UngVienId = uv.MaUngVien,
                    PhongPhongVanId = phongIds[rnd.Next(phongIds.Count)],
                    ViTriId = viTriIds[rnd.Next(viTriIds.Count)],
                    ThoiGian = isVong2
                        ? DateTime.Now.AddDays(rnd.Next(2, 60)).AddHours(rnd.Next(8, 17))
                        : DateTime.Now.AddDays(-rnd.Next(10, 90)).AddHours(rnd.Next(8, 17)),
                    TrangThai = isVong2
                        ? TrangThaiPhongVanEnum.DaLenLich.ToString()
                        : TrangThaiPhongVanEnum.HoanThanh.ToString(),
                    GhiChu = null
                };

                lichList.Add(lich);

                var assignedInterviewer = interviewers[(stt - 1) % interviewers.Count];
                thamGiaList.Add(new NhanVienThamGiaPhongVan
                {
                    Id = $"NVTG{stt:0000}",
                    LichPhongVanId = lich.Id,
                    NhanVienId = assignedInterviewer,
                    VaiTro = "Chủ trì"
                });

                stt++;
            }

            context.LichPhongVans.AddRange(lichList);
            context.NhanVienThamGiaPhongVans.AddRange(thamGiaList);
            context.SaveChanges();
            Console.WriteLine($"✅ Đã seed {lichList.Count} lịch phỏng vấn + {thamGiaList.Count} người phỏng vấn (1:1).");
        }
    }
}
