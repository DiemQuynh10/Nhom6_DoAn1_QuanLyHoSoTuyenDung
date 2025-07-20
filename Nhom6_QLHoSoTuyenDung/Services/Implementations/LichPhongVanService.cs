using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM;

public class LichPhongVanService : ILichPhongVanService
{
    private readonly AppDbContext _context;

    public LichPhongVanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LichPhongVan?> GetLichByUngVienIdAsync(string ungVienId)
    {
        return await _context.LichPhongVans
            .Include(l => l.PhongPhongVan)
            .Include(l => l.UngVien)
            .FirstOrDefaultAsync(l => l.UngVienId == ungVienId);
    }

    public async Task<TaoLichPhongVanVM?> GetFormDataAsync(string ungVienId)
    {
        var ungVien = await _context.UngViens
            .Include(u => u.ViTriUngTuyen)
            .FirstOrDefaultAsync(u => u.MaUngVien == ungVienId);

        if (ungVien == null) return null;

        var phongList = await _context.PhongPhongVans
            .Select(p => new SelectListItem
            {
                Value = p.Id,
                Text = p.TenPhong + " - " + p.DiaDiem
            }).ToListAsync();

        return new TaoLichPhongVanVM
        {
            UngVienId = ungVien.MaUngVien,
            TenUngVien = ungVien.HoTen,
            ViTriId = ungVien.ViTriUngTuyenId,
            TenViTri = ungVien.ViTriUngTuyen?.TenViTri,
            PhongList = phongList
        };
    }


    public async Task<(bool, string)> CreateLichAsync(LichPhongVan model)
    {
        var ungVien = await _context.UngViens.FirstOrDefaultAsync(u => u.MaUngVien == model.UngVienId);
        if (ungVien == null)
            return (false, "Ứng viên không tồn tại.");

        model.ViTriId = ungVien.ViTriUngTuyenId;
        model.Id = Guid.NewGuid().ToString();

        // ✅ Kiểm tra thời gian null hoặc quá khứ
        if (!model.ThoiGian.HasValue)
            return (false, "Vui lòng chọn thời gian hợp lệ.");

        if (model.ThoiGian < DateTime.Now)
            return (false, "Không thể tạo lịch với thời gian trong quá khứ.");

        // ✅ Kiểm tra trùng phòng
        var lichCungPhong = await _context.LichPhongVans
            .Where(l => l.PhongPhongVanId == model.PhongPhongVanId)
            .ToListAsync();

        var clashPhong = lichCungPhong.Any(l =>
            l.ThoiGian.HasValue && model.ThoiGian.HasValue &&
            Math.Abs((l.ThoiGian.Value - model.ThoiGian.Value).TotalMinutes) < 30
        );

        if (clashPhong)
            return (false, "Phòng đã có lịch phỏng vấn gần thời gian này. Vui lòng chọn thời gian khác.");

        // ✅ Kiểm tra trùng người phỏng vấn
        var nhanVienIds = model.NhanVienThamGiaPVs?.Select(n => n.NhanVienId).ToList() ?? new List<string>();

        if (nhanVienIds.Count > 0)
        {
            var lichTrungNhanVien = await _context.LichPhongVans
     .Include(l => l.NhanVienThamGiaPVs)
     .Where(l => l.ThoiGian.HasValue) // Chỉ lấy lịch có thời gian
     .ToListAsync(); // ⛔ phải đưa ToListAsync() sớm để xử lý trong bộ nhớ

            var thoiGianMoi = model.ThoiGian ?? DateTime.MinValue;

            bool clashNhanVien = lichTrungNhanVien.Any(l =>
                Math.Abs((l.ThoiGian!.Value - thoiGianMoi).TotalMinutes) < 30 &&
                l.NhanVienThamGiaPVs.Any(nv => nhanVienIds.Contains(nv.NhanVienId))
            );
            if (clashNhanVien)
                return (false, "Một trong các người phỏng vấn đã có lịch gần thời gian này.");

        }

        // ✅ Lưu lịch nếu không trùng
        _context.LichPhongVans.Add(model);
        await _context.SaveChangesAsync();

        return (true, "Đã tạo lịch phỏng vấn thành công!");
    }


    public async Task<PhongVanDashboardVM> GetDashboardAsync()
    {
        var lich = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .Include(l => l.ViTriTuyenDung)
            .Include(l => l.PhongPhongVan)
            .ToListAsync();

        var model = new PhongVanDashboardVM
        {
            TongSoLich = lich.Count,
            DaPhongVan = lich.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString()),
            ChuaPhongVan = lich.Count(l => l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString()),
            DanhSachLich = lich,

            // ✅ Biểu đồ cột: Vị trí tuyển dụng
            ViTriLabels = lich
                .Where(l => l.ViTriTuyenDung != null)
                .Select(l => l.ViTriTuyenDung.TenViTri)
                .Distinct()
                .ToList(),

            ViTriCounts = lich
                .Where(l => l.ViTriTuyenDung != null)
                .GroupBy(l => l.ViTriTuyenDung.TenViTri)
                .Select(g => g.Count())
                .ToList()
        };

        // ✅ Biểu đồ tròn: Trạng thái phỏng vấn
        var trangThaiGroup = lich
            .GroupBy(l => l.TrangThai)
            .ToList();

        model.TrangThaiLabels = trangThaiGroup
            .Select(g => g.Key ?? "Không xác định")
            .ToList();

        model.TrangThaiValues = trangThaiGroup
            .Select(g => g.Count())
            .ToList();
        model.LichPhongVanSapToi = await GetLichPhongVanSapToiAsync();
        return model;
    }
    public async Task<List<LichPhongVanSapToiVM>> GetLichPhongVanSapToiAsync()
    {
        var lich = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .Include(l => l.ViTriTuyenDung)
            .Where(l => l.ThoiGian.HasValue && l.ThoiGian > DateTime.Now)
            .OrderBy(l => l.ThoiGian)
            .Take(10) // Giới hạn hiển thị
            .ToListAsync();

        return lich.Select(l => new LichPhongVanSapToiVM
        {
            HoTen = l.UngVien?.HoTen,
            ViTri = l.ViTriTuyenDung?.TenViTri,
            Gio = l.ThoiGian!.Value.ToString("HH:mm"),
            Ngay = l.ThoiGian.Value.ToString("dd/MM")
        }).ToList();
    }


    public async Task<List<UngVien>> GetUngViensChuaCoLichAsync()
    {
        var daCoLich = await _context.LichPhongVans
            .Select(l => l.UngVienId)
            .ToListAsync();

        var chuaCoLich = await _context.UngViens
            .Where(u => !daCoLich.Contains(u.MaUngVien))
            .ToListAsync();

        return chuaCoLich;
    }


}
