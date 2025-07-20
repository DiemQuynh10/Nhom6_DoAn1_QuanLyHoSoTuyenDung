using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ViTriTuyenDungVM;

namespace Nhom6_QLHoSoTuyenDung.Models.Helpers
{
    public class ThongKeViTriHelper
    {
        // 1. Phân bố trạng thái vị trí
        public static Dictionary<string, int> DemTheoTrangThai(List<ViTriTuyenDung> ds)
        {
            return ds.GroupBy(v => v.TrangThai ?? "Không rõ")
                     .ToDictionary(g => g.Key, g => g.Count());
        }

        // 2. Thống kê vị trí tạo mới theo tháng
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

        // 3. Thống kê trạng thái "Hoàn thành" theo tháng
        public static List<int> DemTrangThaiHoanThanhTheoThang(List<ViTriTuyenDung> ds)
        {
            return ds.Where(v => v.TrangThai == "Đã đóng" && v.NgayTao.HasValue)
                     .GroupBy(v => v.NgayTao.Value.Month)
                     .OrderBy(g => g.Key)
                     .Select(g => g.Count())
                     .ToList();
        }

        // 4. Thống kê quy trình tuyển dụng dựa trên dữ liệu thật
        public static List<QuyTrinhTuyenDungItem> ThongKeQuyTrinhTuyenDung(AppDbContext context, List<UngVien> ungViens)
        {
            DateTime dauTuan = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime cuoiTuan = dauTuan.AddDays(6);

            int hoSoMoi = ungViens.Count(u => u.TrangThai == TrangThaiUngVienEnum.Moi.ToString()
                                           && u.NgayNop >= dauTuan && u.NgayNop <= cuoiTuan);

            int daLenLich = context.LichPhongVans
        .Count(l => l.TrangThai == TrangThaiPhongVanEnum.DaLenLich.ToString());

            int hoanThanh = context.LichPhongVans
                .Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());

            int daTuyen = ungViens.Count(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            int tuChoi = ungViens.Count(u => u.TrangThai == TrangThaiUngVienEnum.TuChoi.ToString());

            return new List<QuyTrinhTuyenDungItem>
    {
        new() { Ten = "Hồ sơ mới", SoLuong = hoSoMoi, GhiChu = "Tuần này", PhanTramThayDoi = 10 },
        new() { Ten = "Đã lên lịch PV", SoLuong = daLenLich, GhiChu = "Chờ phỏng vấn", PhanTramThayDoi = 4 },
        new() { Ten = "Đã phỏng vấn", SoLuong = hoanThanh, GhiChu = "Hoàn tất phỏng vấn", PhanTramThayDoi = 5 },
        new() { Ten = "Đã tuyển", SoLuong = daTuyen, GhiChu = "Tiếp nhận", PhanTramThayDoi = 3 },
        new() { Ten = "Từ chối", SoLuong = tuChoi, GhiChu = "Không phù hợp", PhanTramThayDoi = -2 }
    };
        }



        // 5. Hoạt động 7 ngày gần nhất
        public static List<HoatDongDashboardVM> LayHoatDong7Ngay(AppDbContext context)
        {
            DateTime tuNgay = DateTime.Now.AddDays(-7);
            var hoatDong = new List<HoatDongDashboardVM>();

            // Vị trí mới tạo
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

            // Ứng viên mới nộp
            var ungVienMoi = context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .Where(u => u.NgayNop >= tuNgay)
                .GroupBy(u => u.ViTriUngTuyen)
                .Select(g => new HoatDongDashboardVM
                {
                    Loai = "upload",
                    TieuDe = "Ứng viên mới",
                    NoiDung = $"{g.Count()} ứng viên nộp hồ sơ cho {g.Key.TenViTri}",
                    Icon = "bi-briefcase",
                    NguoiThucHien = g.Key.TenViTri,
                    ThoiGian = g.Max(u => u.NgayNop) ?? DateTime.Now
                }).ToList();
            hoatDong.AddRange(ungVienMoi);

            // Lịch phỏng vấn được đặt
            var lichPhongVan = context.LichPhongVans
                .Include(l => l.UngVien)
                .Where(l => l.ThoiGian >= tuNgay)
                .Select(l => new HoatDongDashboardVM
                {
                    Loai = "schedule",
                    TieuDe = "Lịch phỏng vấn",
                    NoiDung = $"Đã đặt lịch phỏng vấn cho {l.UngVien.HoTen}",
                    Icon = "bi-calendar-event",
                    NguoiThucHien = l.ThoiGian.HasValue
                        ? l.ThoiGian.Value.ToString("dd/MM/yyyy - HH:mm")
                        : "Chưa rõ",
                    ThoiGian = l.ThoiGian ?? DateTime.Now
                }).ToList();
            hoatDong.AddRange(lichPhongVan);

            // Tuyển dụng hoàn thành
            var daTuyen = context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .Where(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString()
                         && u.NgayNop >= tuNgay)
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

            return hoatDong.OrderByDescending(h => h.ThoiGian).ToList();
        }
    }
}
