using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Services.Implementations
{
    public class NguoiPhongVanService : INguoiPhongVanService
    {
        private readonly AppDbContext _context;

        public NguoiPhongVanService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardNguoiPhongVanVM> GetDashboardAsync(string username)
        {
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(nd => nd.TenDangNhap == username);
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
    .Where(l => l.ThoiGian.HasValue && l.ThoiGian > DateTime.Now && (l.TrangThai == null || l.TrangThai != "Hoàn thành"))
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
        DiaDiem = l.PhongPhongVan?.DiaDiem ?? ""
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
                    var thoiGianTruoc = span.TotalDays >= 1 ? $"{(int)span.TotalDays} ngày trước" :
                                        span.TotalHours >= 1 ? $"{(int)span.TotalHours} giờ trước" :
                                        $"{(int)span.TotalMinutes} phút trước";

                    if (l.TrangThai == "Hoàn thành")
                    {
                        hoatDong.NoiDung = "Phỏng vấn hoàn thành";
                        hoatDong.BieuTuong = "bi-check-circle-fill";
                        hoatDong.Mau = "success";
                    }
                    else if (l.TrangThai == "Đã huỷ")
                    {
                        hoatDong.NoiDung = "Lịch được hoãn";
                        hoatDong.BieuTuong = "bi-x-circle";
                        hoatDong.Mau = "warning";
                    }
                    else if (span.TotalHours <= 12)
                    {
                        hoatDong.NoiDung = "Lịch mới được tạo";
                        hoatDong.BieuTuong = "bi-calendar-plus";
                        hoatDong.Mau = "primary";
                    }
                    else
                    {
                        hoatDong.NoiDung = "Lịch đã lên lịch";
                        hoatDong.BieuTuong = "bi-clock";
                        hoatDong.Mau = "info";
                    }

                    hoatDong.ThoiGianTruoc = thoiGianTruoc;
                    return hoatDong;
                })
                .ToList();

            return new DashboardNguoiPhongVanVM
            {
                TongSoPhongVan = lichPhongVan.Count,
                TangTruongThangTruoc = 2,
                TyLeThanhCong = 60,
                ThoiGianTB = 40,
                ThayDoiThoiGianTB = "+3 phút",
                SoPhongVanHomNay = lichHomNay.Count,
                ThoiGianPhongVanTiepTheo = lichHomNay.Skip(1).FirstOrDefault() != null
                    ? (int)(lichHomNay.Skip(1).First().GioBatDau - DateTime.Now).TotalMinutes
                    : 0,
                PhongVanLabels = new List<string> { "T2", "T3", "T4", "T5", "T6" },
                PhongVanValues = new List<int> { 1, 2, 3, 1, 2 },
                ThanhCongValues = new List<int> { 1, 2, 1, 1, 2 },
                LichHomNay = lichHomNay,
                LichPhongVanSapToi = lichSapToi,
                HoatDongGanDay = hoatDongGanDay
            };
        }
    }
}
