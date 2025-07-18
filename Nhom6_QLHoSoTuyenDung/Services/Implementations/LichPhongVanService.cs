using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
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

    // ✅ BỔ SUNG: Kiểm tra thời gian có null không
    if (!model.ThoiGian.HasValue)
        return (false, "Vui lòng chọn thời gian hợp lệ.");

    if (model.ThoiGian < DateTime.Now)
        return (false, "Không thể tạo lịch với thời gian trong quá khứ.");

    var lichCungPhong = await _context.LichPhongVans
        .Where(l => l.PhongPhongVanId == model.PhongPhongVanId)
        .ToListAsync();

    var clash = lichCungPhong.Any(l =>
        l.ThoiGian.HasValue && model.ThoiGian.HasValue &&
        Math.Abs((l.ThoiGian.Value - model.ThoiGian.Value).TotalMinutes) < 30
    );

    if (clash)
        return (false, "Phòng đã có lịch phỏng vấn gần thời gian này. Vui lòng chọn thời gian khác.");

    _context.LichPhongVans.Add(model);
    await _context.SaveChangesAsync();

    return (true, "Đã tạo lịch phỏng vấn thành công!");
}

    public async Task<LichPhongVanDashboardVM> GetDashboardAsync()
    {
        // 1. Lấy toàn bộ danh sách để thống kê đúng
        var allLich = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .Include(l => l.ViTriTuyenDung)
            .Include(l => l.PhongPhongVan)
            .ToListAsync();

        // 2. Chỉ lấy lịch đang chờ để hiển thị trong dashboard (không gồm đã tiếp nhận/hủy)
        var lichHienThi = allLich
            .Where(l => l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString()
                     && l.TrangThai != TrangThaiPhongVanEnum.Huy.ToString())
            .ToList();

        // 3. Tạo model
        var model = new LichPhongVanDashboardVM
        {
            TongSoLich = allLich.Count,
            DaPhongVan = allLich.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString()),
            ChuaPhongVan = allLich.Count(l => l.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString()),
            DanhSachLich = lichHienThi,
            ViTriLabels = lichHienThi.Select(l => l.ViTriTuyenDung.TenViTri).Distinct().ToList(),
            ViTriCounts = lichHienThi
                .GroupBy(l => l.ViTriTuyenDung.TenViTri)
                .Select(g => g.Count())
                .ToList()
        };

        return model;
    }


}
