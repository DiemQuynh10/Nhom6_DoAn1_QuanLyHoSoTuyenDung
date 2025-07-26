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

        public async Task<ThongKeTongHopVM> GetTongQuanAsync(string? tuKhoa, string? loai, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay);

            var tong = await filtered.CountAsync();
            var daTuyen = await filtered.CountAsync(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());
            var xuLy = await filtered.CountAsync(u =>
                u.TrangThai == TrangThaiUngVienEnum.Moi.ToString()
                || u.TrangThai == TrangThaiUngVienEnum.DaPhongVan.ToString()
                || u.TrangThai == TrangThaiUngVienEnum.CanPhongVanLan2.ToString()
            );

            var viTriDangTuyen = await _context.ViTriTuyenDungs
                .Where(v => v.TrangThai == "Đang tuyển")
                .CountAsync();

            var daTuyenList = await filtered
                .Where(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString())
                .Include(u => u.ViTriUngTuyen)
                .Select(u => new UngVienTuyenDungVM
                {
                    HoTen = u.HoTen,
                    Email = u.Email,
                    TenViTri = u.ViTriUngTuyen.TenViTri,
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


        // 2. Biểu đồ theo trạng thái ứng viên
        public async Task<List<BieuDoItemVM>> GetBieuDoTheoTrangThaiUngVienAsync()
        {
            return await _context.UngViens
                .GroupBy(u => u.TrangThai ?? "Khác")
                .Select(g => new BieuDoItemVM
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                }).ToListAsync();
        }

        // 3. Biểu đồ theo nguồn ứng viên
        public async Task<List<BieuDoItemVM>> GetBieuDoNguonUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = await FilterUngViens(tuKhoa, tuNgay, denNgay).ToListAsync();

            var result = filtered
                .GroupBy(u =>
                {
                    var nguon = u.NguonUngTuyen?.Trim().ToLower();
                    if (nguon?.Contains("linkedin") == true) return "LinkedIn";
                    if (nguon?.Contains("website") == true) return "Website công ty";
                    if (nguon?.Contains("giới thiệu") == true || nguon?.Contains("gioi thieu") == true) return "Giới thiệu";
                    return "Khác";
                })
                .Select(g => new BieuDoItemVM
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(g => g.SoLuong)
                .ToList();

            return result;
        }



        // 4. Biểu đồ theo vị trí
        public async Task<List<BieuDoItemVM>> GetBieuDoTheoViTriUngTuyenAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay)
                .Include(u => u.ViTriUngTuyen);

            return await filtered
                .GroupBy(u => u.ViTriUngTuyen!.TenViTri)
                .Select(g => new BieuDoItemVM
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                }).ToListAsync();
        }


        // 5. Biểu đồ theo phòng ban
        public async Task<List<BieuDoItemVM>> GetBieuDoTheoPhongBanAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay)
                .Include(u => u.ViTriUngTuyen).ThenInclude(v => v.PhongBan);

            return await filtered
                .GroupBy(u => u.ViTriUngTuyen!.PhongBan!.TenPhong)
                .Select(g => new BieuDoItemVM
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                }).ToListAsync();
        }


        // 6. Biểu đồ điểm đánh giá
        public async Task<List<BieuDoItemVM>> GetBieuDoDanhGiaUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.DanhGiaPhongVans
                .Include(d => d.LichPhongVan).ThenInclude(lp => lp.UngVien)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                query = query.Where(d =>
                    d.LichPhongVan!.UngVien!.HoTen.Contains(tuKhoa) ||
                    d.LichPhongVan.UngVien.Email.Contains(tuKhoa));

            if (tuNgay.HasValue)
                query = query.Where(d =>
                    d.LichPhongVan!.UngVien!.NgayNop.HasValue &&
                    d.LichPhongVan.UngVien.NgayNop.Value.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                query = query.Where(d =>
                    d.LichPhongVan!.UngVien!.NgayNop.HasValue &&
                    d.LichPhongVan.UngVien.NgayNop.Value.Date <= denNgay.Value.Date);

            return await query
                .GroupBy(d =>
                    d.DiemDanhGia == null ? "Chưa đánh giá" :
                    d.DiemDanhGia < 5 ? "Yếu" :
                    d.DiemDanhGia < 7 ? "Trung bình" :
                    d.DiemDanhGia < 8.5 ? "Khá" : "Tốt"
                )
                .Select(g => new BieuDoItemVM
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                }).ToListAsync();
        }



        // 7. Xu hướng theo tháng (tính theo NgayNop của UngVien)
        public async Task<List<BieuDoItemVM>> GetXuHuongTheoThangAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay)
                .Where(u => u.NgayNop != null);

            return await filtered
                .GroupBy(u => u.NgayNop!.Value.Month)
                .Select(g => new BieuDoItemVM
                {
                    Ten = $"Tháng {g.Key}",
                    SoLuong = g.Count()
                }).ToListAsync();
        }

        // 8. Vị trí đã tuyển thành công (Top 5)
        public async Task<List<ViTriThanhCongVM>> GetViTriTuyenThanhCongAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay)
                .Include(u => u.ViTriUngTuyen).ThenInclude(v => v.PhongBan)
                .Where(u => u.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString());

            return await filtered
                .GroupBy(u => new
                {
                    TenViTri = u.ViTriUngTuyen!.TenViTri,
                    PhongBan = u.ViTriUngTuyen.PhongBan!.TenPhong
                })
                .Select(g => new ViTriThanhCongVM
                {
                    TenViTri = g.Key.TenViTri,
                    PhongBan = g.Key.PhongBan,
                    SoLuongTuyen = g.Count(),
                    NgayTuyenGanNhat = g.Max(u => u.NgayNop)!.Value.ToString("dd/MM/yyyy")
                })
                .OrderByDescending(v => v.SoLuongTuyen)
                .Take(5)
                .ToListAsync();
        }

        private IQueryable<UngVien> FilterUngViens(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.UngViens.AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
                query = query.Where(u => u.HoTen.Contains(tuKhoa) || u.Email.Contains(tuKhoa));

            if (tuNgay.HasValue)
                query = query.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Date >= tuNgay.Value.Date);

            if (denNgay.HasValue)
                query = query.Where(u => u.NgayNop.HasValue && u.NgayNop.Value.Date <= denNgay.Value.Date);

            return query;
        }
        public async Task<List<BieuDoItemVM>> GetBieuDoTheoTrangThaiUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = FilterUngViens(tuKhoa, tuNgay, denNgay);

            return await filtered
                .GroupBy(u => u.TrangThai ?? "Khác")
                .Select(g => new BieuDoItemVM
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                }).ToListAsync();
        }


    }
}
