using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ViTriTuyenDungVM;

namespace Nhom6_QLHoSoTuyenDung.Models.Helpers
{
    public class ThongKeViTriHelper
    {
        // Phân bố trạng thái
        public static Dictionary<string, int> DemTheoTrangThai(List<ViTriTuyenDung> ds)
        {
            return ds.GroupBy(v => v.TrangThai ?? "Không rõ")
                     .ToDictionary(g => g.Key, g => g.Count());
        }

        // Phân bố theo tháng (xu hướng tạo mới)
        public static (List<string> Thang, List<int> SoLuong) DemTheoThang(List<ViTriTuyenDung> ds)
        {
            var thangData = ds.Where(v => v.NgayTao.HasValue)
                              .GroupBy(v => v.NgayTao.Value.Month)
                              .OrderBy(g => g.Key)
                              .ToList();

            var thang = thangData.Select(g => $"T{g.Key}").ToList();
            var soLuong = thangData.Select(g => g.Count()).ToList();

            return (thang, soLuong);
        }

        // (Tuỳ chọn) Nếu có trạng thái "Hoàn thành"
        public static List<int> DemTrangThaiHoanThanhTheoThang(List<ViTriTuyenDung> ds)
        {
            return ds.Where(v => v.TrangThai == "Hoàn thành" && v.NgayTao.HasValue)
                     .GroupBy(v => v.NgayTao.Value.Month)
                     .OrderBy(g => g.Key)
                     .Select(g => g.Count())
                     .ToList();
        }
        public static List<QuyTrinhTuyenDungItem> ThongKeQuyTrinhTuyenDung(List<UngVien> ungViens)
        {
            DateTime dauTuan = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime cuoiTuan = dauTuan.AddDays(6);

            int hoSoMoi = ungViens.Count(u => u.NgayNop >= dauTuan && u.NgayNop <= cuoiTuan);
            int dangXemXet = ungViens.Count(u => u.TrangThai == "Đang xem xét");
            int phongVan = ungViens.Count(u => u.TrangThai == "Phỏng vấn");
            int deXuat = ungViens.Count(u => u.TrangThai == "Đề xuất");

            return new List<QuyTrinhTuyenDungItem>
    {
        new() { Ten = "Hồ sơ mới", SoLuong = hoSoMoi, GhiChu = "Tuần này", PhanTramThayDoi = 15 },
        new() { Ten = "Đang xem xét", SoLuong = dangXemXet, GhiChu = "Chờ duyệt", PhanTramThayDoi = -5 },
        new() { Ten = "Phỏng vấn", SoLuong = phongVan, GhiChu = "Đã lên lịch", PhanTramThayDoi = 8 },
        new() { Ten = "Đề xuất", SoLuong = deXuat, GhiChu = "Chờ quyết định", PhanTramThayDoi = 3 },
    };
        }

        public static List<HoatDongDashboardVM> LayHoatDong7Ngay(AppDbContext context)
        {
            DateTime tuNgay = DateTime.Now.AddDays(-7);
            var hoatDong = new List<HoatDongDashboardVM>();

            // 1. Vị trí mới tạo
            var viTriMoi = context.ViTriTuyenDungs
                .Include(v => v.PhongBan)
                .Where(v => v.NgayTao >= tuNgay)
                .Select(v => new HoatDongDashboardVM
                {
                    Loai = "create",
                    TieuDe = "Vị trí mới được tạo",
                    NoiDung = $"{v.TenViTri} đã được đăng tuyển",
                    Icon = "bi-person-workspace",
                    NguoiThucHien = v.PhongBan != null ? v.PhongBan.TenPhong : "Phòng ban",
                    ThoiGian = v.NgayTao ?? DateTime.Now
                }).ToList();

            hoatDong.AddRange(viTriMoi);

            // 2. Ứng viên mới nộp
            var ungVienMoi = context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .Where(u => u.NgayNop >= tuNgay)
                .GroupBy(u => u.ViTriUngTuyen)
                .Select(g => new HoatDongDashboardVM
                {
                    Loai = "upload",
                    TieuDe = "Ứng viên mới",
                    NoiDung = $"{g.Count()} ứng viên mới nộp hồ sơ cho {g.Key.TenViTri}",
                    Icon = "bi-briefcase",
                    NguoiThucHien = g.Key.TenViTri,
                    ThoiGian = g.Max(u => u.NgayNop) ?? DateTime.Now
                }).ToList();

            hoatDong.AddRange(ungVienMoi);

            // 3. Lịch phỏng vấn được đặt
            var lichPhongVan = context.LichPhongVans
                .Include(l => l.UngVien)
                .Where(l => l.ThoiGian >= tuNgay)
                .Select(l => new HoatDongDashboardVM
                {
                    Loai = "schedule",
                    TieuDe = "Lịch phỏng vấn",
                    NoiDung = $"Đã đặt lịch phỏng vấn cho {l.UngVien.HoTen}",
                    Icon = "bi bi-calendar-event",
                    NguoiThucHien = l.ThoiGian.HasValue
    ? l.ThoiGian.Value.ToString("dd/MM/yyyy - HH:mm")
    : "Chưa rõ thời gian",
                    ThoiGian = l.ThoiGian ?? DateTime.Now
                }).ToList();

            hoatDong.AddRange(lichPhongVan);

            // 4. Tuyển dụng hoàn thành
            var daTuyen = context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .Where(u => u.TrangThai == "Đã tuyển" && u.NgayNop >= tuNgay)
                .Select(u => new HoatDongDashboardVM
                {
                    Loai = "complete",
                    TieuDe = "Hoàn thành tuyển dụng",
                    NoiDung = $"{u.ViTriUngTuyen.TenViTri} đã tuyển được ứng viên phù hợp",
                    Icon = "bi-check-circle",
                    NguoiThucHien = u.HoTen,
                    ThoiGian = u.NgayNop ?? DateTime.Now
                }).ToList();

            hoatDong.AddRange(daTuyen);

            // Sắp xếp theo thời gian mới nhất
            return hoatDong.OrderByDescending(h => h.ThoiGian).ToList();
        }

    }
}
