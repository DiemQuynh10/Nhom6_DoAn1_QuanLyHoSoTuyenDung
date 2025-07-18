using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

public class DanhGiaPhongVanService : IDanhGiaPhongVanService
{
    private readonly AppDbContext _context;

    public DanhGiaPhongVanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DanhGiaPhongVanVM?> GetFormAsync(string lichId)
    {
        var lich = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .Include(l => l.ViTriTuyenDung)
            .Include(l => l.PhongPhongVan)
            .FirstOrDefaultAsync(l => l.Id == lichId);

        if (lich == null) return null;

        return new DanhGiaPhongVanVM
        {
            LichPhongVanId = lich.Id,
            TenUngVien = lich.UngVien?.HoTen,
            TenViTri = lich.ViTriTuyenDung?.TenViTri,
            TenPhong = lich.PhongPhongVan?.TenPhong,
            ThoiGian = lich.ThoiGian ?? DateTime.Now,
            KinhNghiem = lich.UngVien?.KinhNghiem
        };

    }

    public async Task<bool> LuuAsync(DanhGiaPhongVanVM vm, string nguoiDungId)
    {
        var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNhanVien == nguoiDungId);
        if (nhanVien == null) return false;

        var danhGia = await _context.DanhGiaPhongVans
            .FirstOrDefaultAsync(d => d.LichPhongVanId == vm.LichPhongVanId && d.NhanVienDanhGiaId == nhanVien.MaNhanVien);

        if (danhGia == null)
        {
            danhGia = new DanhGiaPhongVan
            {
                Id = Guid.NewGuid().ToString(),
                LichPhongVanId = vm.LichPhongVanId,
                NhanVienDanhGiaId = nhanVien.MaNhanVien
            };
            _context.DanhGiaPhongVans.Add(danhGia);
        }

        // Cập nhật nội dung
        danhGia.DiemDanhGia = (int)vm.DiemDanhGia;
        danhGia.NhanXet = vm.NhanXet;
        if (vm.DeXuat.HasValue)
        {
            danhGia.DeXuat = vm.DeXuat.Value.ToString();
        }

        await _context.SaveChangesAsync();
        return true;
    }




}
