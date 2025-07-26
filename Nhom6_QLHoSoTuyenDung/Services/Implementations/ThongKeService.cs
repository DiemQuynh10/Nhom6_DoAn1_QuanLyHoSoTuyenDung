using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ThongKe;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using static Nhom6_QLHoSoTuyenDung.Models.ViewModels.ThongKe.ThongKeTongHopVM;

namespace Nhom6_QLHoSoTuyenDung.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly AppDbContext _context;

        public ThongKeService(AppDbContext context)
        {
            _context = context;
        }

        private IQueryable<UngVien> FilterUngViens(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, string? trangThai = null, string? viTriId = null, string? phongBanId = null)
        {
            var query = _context.UngViens.Include(u => u.ViTriUngTuyen).ThenInclude(v => v.PhongBan).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                query = query.Where(u => u.HoTen.Contains(tuKhoa) || u.Email.Contains(tuKhoa));

            if (tuNgay.HasValue && denNgay.HasValue)
                query = query.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Date >= tuNgay.Value.Date && u.NgayNop.Value.Date <= denNgay.Value.Date);
            else if (tuNgay.HasValue)
                query = query.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Date >= tuNgay.Value.Date);
            else if (denNgay.HasValue)
                query = query.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Date <= denNgay.Value.Date);

            if (!string.IsNullOrWhiteSpace(trangThai))
                query = query.Where(u => u.TrangThai == trangThai);

            if (!string.IsNullOrWhiteSpace(viTriId))
                query = query.Where(u => u.ViTriUngTuyenId == viTriId);

            if (!string.IsNullOrWhiteSpace(phongBanId))
                query = query.Where(u => u.ViTriUngTuyen!.PhongBanId == phongBanId);

            return query;
        }

        public async Task<ThongKeTongHopVM> GetTongQuanAsync(string? tuKhoa, string? loai, DateTime? tuNgay, DateTime? denNgay, string? trangThai = null, string? viTriId = null, string? phongBanId = null)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId);

            var tong = await filtered.CountAsync();
            var daTuyen = await filtered.CountAsync(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            var xuLy = await filtered.CountAsync(u =>
                u.TrangThai == TrangThaiUngVienEnum.Moi.ToString()
                || u.TrangThai == TrangThaiUngVienEnum.DaPhongVan.ToString()
                || u.TrangThai == TrangThaiUngVienEnum.CanPhongVanLan2.ToString());

            var viTriDangTuyen = await _context.ViTriTuyenDungs.Where(v => v.TrangThai == "Đang tuyển").CountAsync();

            var daTuyenList = await filtered
                .Where(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString())
                .Select(u => new UngVienTuyenDungVM
                {
                    HoTen = u.HoTen,
                    Email = u.Email,
                    TenViTri = u.ViTriUngTuyen!.TenViTri,
                    NgayNop = u.NgayNop ?? DateTime.MinValue
                })
                .ToListAsync();

            return new ThongKeTongHopVM
            {
                TongUngVien = tong,
                SoDaTuyen = daTuyen,
                SoDangXuLy = xuLy,
                SoViTriDangTuyen = viTriDangTuyen,
                UngVienDaTuyen = daTuyenList,
                ThoiGianTuyenTrungBinhNgay = 0
            };
        }

        public async Task<List<BieuDoItemVM>> GetBieuDoTheoTrangThaiUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, string? trangThai = null, string? viTriId = null, string? phongBanId = null)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId);

            return await filtered
                .GroupBy(u => u.TrangThai ?? "Khác")
                .Select(g => new BieuDoItemVM { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<List<BieuDoItemVM>> GetBieuDoNguonUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, string? trangThai = null, string? viTriId = null, string? phongBanId = null)
        {
            var filtered = await FilterUngViens(tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId).ToListAsync();

            return filtered
                .GroupBy(u =>
                {
                    var nguon = u.NguonUngTuyen?.Trim().ToLower();
                    if (nguon?.Contains("linkedin") == true) return "LinkedIn";
                    if (nguon?.Contains("website") == true) return "Website công ty";
                    if (nguon?.Contains("giới thiệu") == true || nguon?.Contains("gioi thieu") == true) return "Giới thiệu";
                    return "Khác";
                })
                .Select(g => new BieuDoItemVM { Ten = g.Key, SoLuong = g.Count() })
                .OrderByDescending(x => x.SoLuong)
                .ToList();
        }

        public async Task<List<BieuDoItemVM>> GetBieuDoTheoViTriUngTuyenAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, string? trangThai, string? viTriId, string? phongBanId)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId).Include(u => u.ViTriUngTuyen);

            return await filtered
                .GroupBy(u => u.ViTriUngTuyen!.TenViTri)
                .Select(g => new BieuDoItemVM { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<List<BieuDoItemVM>> GetBieuDoTheoPhongBanAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, string? trangThai, string? viTriId, string? phongBanId)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId)
                .Include(u => u.ViTriUngTuyen).ThenInclude(v => v.PhongBan);

            return await filtered
                .GroupBy(u => u.ViTriUngTuyen!.PhongBan!.TenPhong)
                .Select(g => new BieuDoItemVM { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<List<BieuDoItemVM>> GetBieuDoDanhGiaUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.DanhGiaPhongVans.Include(d => d.LichPhongVan).ThenInclude(lp => lp.UngVien).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                query = query.Where(d => d.LichPhongVan!.UngVien!.HoTen.Contains(tuKhoa) || d.LichPhongVan.UngVien.Email.Contains(tuKhoa));

            if (tuNgay.HasValue && denNgay.HasValue)
                query = query.Where(d => d.LichPhongVan!.UngVien!.NgayNop.HasValue && d.LichPhongVan.UngVien.NgayNop.Value.Date >= tuNgay.Value.Date && d.LichPhongVan.UngVien.NgayNop.Value.Date <= denNgay.Value.Date);
            else if (tuNgay.HasValue)
                query = query.Where(d => d.LichPhongVan!.UngVien!.NgayNop.HasValue && d.LichPhongVan.UngVien.NgayNop.Value.Date >= tuNgay.Value.Date);
            else if (denNgay.HasValue)
                query = query.Where(d => d.LichPhongVan!.UngVien!.NgayNop.HasValue && d.LichPhongVan.UngVien.NgayNop.Value.Date <= denNgay.Value.Date);

            return await query.GroupBy(d =>
                d.DiemDanhGia == null ? "Chưa đánh giá" :
                d.DiemDanhGia < 5 ? "Yếu" :
                d.DiemDanhGia < 7 ? "Trung bình" :
                d.DiemDanhGia < 8.5 ? "Khá" : "Tốt")
                .Select(g => new BieuDoItemVM { Ten = g.Key, SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<List<BieuDoItemVM>> GetXuHuongTheoThangAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay, string? trangThai, string? viTriId, string? phongBanId)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay, trangThai, viTriId, phongBanId)
                .Where(u => u.NgayNop != null);

            return await filtered
                .GroupBy(u => new { Thang = u.NgayNop!.Value.Month, Nam = u.NgayNop.Value.Year })
                .Select(g => new BieuDoItemVM { Ten = $"Tháng {g.Key.Thang}/{g.Key.Nam}", SoLuong = g.Count() })
                .ToListAsync();
        }

        public async Task<List<ViTriThanhCongVM>> GetViTriTuyenThanhCongAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay, TrangThaiUngVienEnum.DaTuyen.ToString())
                .Include(u => u.ViTriUngTuyen).ThenInclude(v => v.PhongBan);

            return await filtered
                .GroupBy(u => new { TenViTri = u.ViTriUngTuyen!.TenViTri, PhongBan = u.ViTriUngTuyen.PhongBan!.TenPhong })
                .Select(g => new ViTriThanhCongVM
                {
                    TenViTri = g.Key.TenViTri,
                    PhongBan = g.Key.PhongBan,
                    SoLuongTuyen = g.Count(),
                    NgayTuyenGanNhat = g.Max(u => u.NgayNop)!.Value.ToString("dd/MM/yyyy")
                })
                .OrderByDescending(x => x.SoLuongTuyen)
                .Take(5)
                .ToListAsync();
        }
        public async Task<List<BaoCaoDayDuVM>> XuatBaoCaoDayDuAsync(BaoCaoRequestVM request)
        {
            var query = _context.UngViens
                .Include(u => u.ViTriUngTuyen)
                    .ThenInclude(v => v.PhongBan)
                .AsQueryable();

            // Lọc theo từ khoá
            if (!string.IsNullOrEmpty(request.TuKhoa))
            {
                query = query.Where(u => u.HoTen.Contains(request.TuKhoa) || u.Email.Contains(request.TuKhoa));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(request.TrangThai))
            {
                query = query.Where(u => u.TrangThai == request.TrangThai);
            }

            // Lọc theo vị trí
            if (!string.IsNullOrEmpty(request.ViTriId))
            {
                query = query.Where(u => u.ViTriUngTuyenId == request.ViTriId);
            }

            // Lọc theo phòng ban
            if (!string.IsNullOrEmpty(request.PhongBanId))
            {
                query = query.Where(u => u.ViTriUngTuyen.PhongBanId == request.PhongBanId);
            }

            // Lọc theo ngày nộp
            if (request.TuNgay.HasValue)
            {
                query = query.Where(u => u.NgayNop >= request.TuNgay.Value);
            }

            if (request.DenNgay.HasValue)
            {
                query = query.Where(u => u.NgayNop <= request.DenNgay.Value);
            }

            var data = await query
                .OrderByDescending(u => u.NgayNop)
                .Select(u => new BaoCaoDayDuVM
                {
                    HoTen = u.HoTen,
                    Email = u.Email,
                    DienThoai = u.SoDienThoai,
                    ViTri = u.ViTriUngTuyen.TenViTri,
                    PhongBan = u.ViTriUngTuyen.PhongBan.TenPhong,
                    NgayNop = u.NgayNop.Value,
                    TrangThai = u.TrangThai
                })
                .ToListAsync();

            return data;
        }

    }
}
