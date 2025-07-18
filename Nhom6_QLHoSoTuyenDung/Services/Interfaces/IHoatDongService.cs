using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Nhom6_QLHoSoTuyenDung.Services
{
    public interface IHoatDongService
    {
        List<HoatDongDashboardVM> GetHoatDongGanDay(string currentUser, DateTime startOfWeek);
    }

    public class HoatDongService : IHoatDongService
    {
        private readonly AppDbContext _context;
        public HoatDongService(AppDbContext context)
        {
            _context = context;
        }

        public List<HoatDongDashboardVM> GetHoatDongGanDay(string currentUser, DateTime startOfWeek)
        {
            var hoatDongs = new List<HoatDongDashboardVM>();
            var tenNguoiThucHien = _context.NguoiDungs.FirstOrDefault(n => n.TenDangNhap == currentUser)?.HoTen ?? "Hệ thống";

            // 1. Hồ sơ mới
            hoatDongs.AddRange(
                _context.UngViens
                    .Include(u => u.ViTriUngTuyen)
                    .Where(u => u.NgayNop.HasValue && u.NgayNop.Value >= startOfWeek)
                    .OrderByDescending(u => u.NgayNop)
                    .Take(5)
                    .Select(u => new HoatDongDashboardVM
                    {
                        TieuDe = "Hồ sơ mới",
                        Loai = "create",
                        UngVien = u.HoTen,
                        ViTri = u.ViTriUngTuyen.TenViTri,
                        NguoiThucHien = tenNguoiThucHien,
                        ThoiGian = u.NgayNop
                    })
            );

            // 2. Mời phỏng vấn
            hoatDongs.AddRange(
                _context.LichPhongVans
                    .Include(l => l.UngVien)
                    .Include(l => l.ViTriTuyenDung)
                    .Where(l => l.ThoiGian.HasValue && l.ThoiGian.Value >= startOfWeek)
                    .OrderByDescending(l => l.ThoiGian)
                    .Take(5)
                    .Select(l => new HoatDongDashboardVM
                    {
                        TieuDe = "Mời phỏng vấn",
                        Loai = "invite",
                        UngVien = l.UngVien.HoTen,
                        ViTri = l.ViTriTuyenDung.TenViTri,
                        NguoiThucHien = tenNguoiThucHien,
                        ThoiGian = l.ThoiGian
                    })
            );

            // 3. Đề nghị, từ chối, đã tuyển
            var trangThaiDacBiet = new[] { "Từ chối", "Đề nghị", "Đã tuyển" };
            hoatDongs.AddRange(
                _context.UngViens
                    .Include(u => u.ViTriUngTuyen)
                    .Where(u => u.NgayNop.HasValue && u.NgayNop.Value >= startOfWeek && trangThaiDacBiet.Contains(u.TrangThai))
                    .OrderByDescending(u => u.NgayNop)
                    .Take(5)
                    .Select(u => new HoatDongDashboardVM
                    {
                        TieuDe = u.TrangThai,
                        Loai = u.TrangThai.ToLower().Replace(" ", ""),
                        UngVien = u.HoTen,
                        ViTri = u.ViTriUngTuyen.TenViTri,
                        NguoiThucHien = tenNguoiThucHien,
                        ThoiGian = u.NgayNop
                    })
            );

            return hoatDongs.OrderByDescending(h => h.ThoiGian).Take(5).ToList();
        }
    }
}
