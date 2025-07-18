using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.Helpers;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Services.Implementations
{
    public class NguoiPhongVanService : INguoiPhongVanService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContext;

        public NguoiPhongVanService(AppDbContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
        }

        public async Task<DashboardNguoiPhongVanVM> GetDashboardAsync(string username)
        {
            var nguoiDung = await _context.NguoiDungs
                .Include(nd => nd.NhanVien)
                .FirstOrDefaultAsync(nd => nd.TenDangNhap == username);

            if (nguoiDung == null) return new DashboardNguoiPhongVanVM();

            var nhanVienId = nguoiDung.NhanVienId;

            var lichPhongVan = await _context.NhanVienThamGiaPhongVans
                .Where(x => x.NhanVienId == nhanVienId && x.LichPhongVan != null)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.UngVien)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.ViTriTuyenDung)
                .Include(x => x.LichPhongVan)
                    .ThenInclude(l => l.PhongPhongVan)
                .Select(x => x.LichPhongVan!)
                .ToListAsync();

            var today = DateTime.Today;
            var weekAgo = DateTime.Now.AddDays(-7);
            var startOfThisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var startOfLastMonth = startOfThisMonth.AddMonths(-1);
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);

            var lichHomNay = lichPhongVan
                .Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value.Date == today)
                .OrderBy(l => l.ThoiGian)
                .Select(l => new LichPhongVanHomNayVM
                {
                    Id = l.Id ?? "",
                    HoTen = l.UngVien?.HoTen ?? "Không có tên",
                    ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                    GioBatDau = l.ThoiGian!.Value,
                    GioKetThuc = l.ThoiGian.Value.AddMinutes(40),
                    HinhThuc = l.PhongPhongVan?.DiaDiem ?? "Online",
                    TrangThai = l.TrangThai ?? "Chưa xác định"
                })
                .ToList();

            var lichSapToi = lichPhongVan
                .Where(l =>
                    l.ThoiGian.HasValue &&
                    l.ThoiGian > DateTime.Now &&
                    l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString() &&
                    l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString()
                )
                .OrderBy(l => l.ThoiGian)
                .Take(5)
                .Select(l => new LichPhongVanVM
                {
                    Id = l.Id ?? "",
                    HoTen = l.UngVien?.HoTen ?? "Không có tên",
                    ViTri = l.ViTriTuyenDung?.TenViTri ?? "Không rõ",
                    ThoiGian = l.ThoiGian,
                    NhanNhan = (l.ThoiGian.Value - DateTime.Now).TotalDays < 1 ? "Hôm nay" : ((l.ThoiGian.Value - DateTime.Now).TotalDays < 2 ? "Ngày mai" : ""),
                    Email = l.UngVien?.Email ?? "",
                    SoDienThoai = l.UngVien?.SoDienThoai ?? "",
                    KinhNghiem = l.UngVien?.KinhNghiem ?? "",
                    TenPhong = l.PhongPhongVan?.TenPhong ?? "Không rõ",
                    DiaDiem = l.PhongPhongVan?.DiaDiem ?? "",
                    TrangThai = l.TrangThai ?? "Chưa xác định"
                })
                .ToList();

            var hoatDongGanDay = lichPhongVan
    .Where(l => (l.ThoiGian ?? DateTime.MinValue) >= weekAgo)
    .OrderByDescending(l => l.ThoiGian ?? DateTime.MinValue)
    .Take(10)
    .Select(l =>
    {
        var hoatDong = new HoatDongGanDayVM();
        var thoiGian = l.ThoiGian ?? DateTime.Now;
        var span = DateTime.Now - thoiGian;
        var thoiGianTruoc = ThoiGianHelper.TinhTuLuc(thoiGian);

        string hoTenUngVien = l.UngVien?.HoTen ?? "Không rõ";
        string viTri = l.ViTriTuyenDung?.TenViTri ?? "Vị trí không rõ";

        if (l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString())
        {
            hoatDong.NoiDung = $"Phỏng vấn đã hoàn thành với {hoTenUngVien} ({viTri})";
            hoatDong.BieuTuong = "bi-check-circle-fill";
            hoatDong.Mau = "success";
        }
        else if (l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString())
        {
            hoatDong.NoiDung = $"Lịch phỏng vấn với {hoTenUngVien} đã bị huỷ";
            hoatDong.BieuTuong = "bi-x-circle-fill";
            hoatDong.Mau = "danger";
        }
        else if (span.TotalHours <= 12)
        {
            hoatDong.NoiDung = $"Tạo lịch phỏng vấn cho {hoTenUngVien} ({viTri})";
            hoatDong.BieuTuong = "bi-calendar-plus-fill";
            hoatDong.Mau = "primary";
        }
        else
        {
            hoatDong.NoiDung = $"Lịch phỏng vấn với {hoTenUngVien} đã được lên lịch";
            hoatDong.BieuTuong = "bi-clock-fill";
            hoatDong.Mau = "info";
        }

        hoatDong.ThoiGianTruoc = thoiGianTruoc;
        return hoatDong;
    })
    .ToList();

            int daHoanThanhHomNay = lichHomNay.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());

            // Tính số phỏng vấn tháng này và tháng trước
            var tongThangNay = lichPhongVan.Count(l => l.ThoiGian >= startOfThisMonth);
            var tongThangTruoc = lichPhongVan.Count(l => l.ThoiGian >= startOfLastMonth && l.ThoiGian < startOfThisMonth);
            int tangTruong = tongThangTruoc == 0 ? tongThangNay : tongThangNay - tongThangTruoc;

            // Tính tỷ lệ thành công theo đề xuất
            int tongHoanThanh = lichPhongVan.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());
            int soThanhCong = lichPhongVan.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString() && l.UngVien?.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            int tyLeThanhCong = tongHoanThanh == 0 ? 0 : (int)Math.Round((double)soThanhCong * 100 / tongHoanThanh);

            // Duyệt qua enum và đếm từng loại
            var labels = new List<string>();
            var counts = new List<int>();

            foreach (TrangThaiPhongVanEnum trangThai in Enum.GetValues(typeof(TrangThaiPhongVanEnum)))
            {
                string displayName = trangThai.GetDisplayName();
                int count = lichPhongVan.Count(l => l.TrangThai == trangThai.ToString());

                labels.Add(displayName);
                counts.Add(count);
            }

            // Dữ liệu biểu đồ xu hướng theo thứ
            var phongVanTheoNgay = new Dictionary<string, int>();
            var thanhCongTheoNgay = new Dictionary<string, int>();

            for (int i = 0; i < 5; i++)
            {
                var day = startOfWeek.AddDays(i);
                string label = "T" + (i + 2);
                var lichTrongNgay = lichPhongVan.Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value.Date == day.Date).ToList();
                phongVanTheoNgay[label] = lichTrongNgay.Count;
                thanhCongTheoNgay[label] = lichTrongNgay.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString() && l.UngVien?.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            }

            return new DashboardNguoiPhongVanVM
            {
                HoTen = nguoiDung.NhanVien?.HoTen ?? nguoiDung.TenDangNhap,
                ChucDanh = nguoiDung.NhanVien?.ChucVu ?? "",

                TongSoPhongVan = lichPhongVan.Count,
                TangTruongThangTruoc = tangTruong,
                TyLeThanhCong = tyLeThanhCong,
                ThoiGianTB = 40,
                ThayDoiThoiGianTB = "+3 phút",
                SoPhongVanHomNay = lichHomNay.Count,
                SoDaHoanThanhHomNay = daHoanThanhHomNay,

                PhongVanLabels = phongVanTheoNgay.Keys.ToList(),
                PhongVanValues = phongVanTheoNgay.Values.ToList(),
                ThanhCongValues = thanhCongTheoNgay.Values.ToList(),

                TrangThaiUngVienLabels = labels,
                TrangThaiUngVienCounts = counts,

                LichHomNay = lichHomNay,
                LichPhongVanSapToi = lichSapToi,
                HoatDongGanDay = hoatDongGanDay
            };
        }
    }
}
