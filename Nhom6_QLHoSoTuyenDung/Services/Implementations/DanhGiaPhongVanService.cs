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

    public async Task<bool> LuuAsync(DanhGiaPhongVanVM vm, int nguoiDungId)
    {
        var nguoiDungIdStr = nguoiDungId.ToString();
        var nhanVien = await _context.NhanViens
            .FirstOrDefaultAsync(n => n.MaNhanVien == nguoiDungIdStr);

        if (nhanVien == null) return false;

        var danhGia = new DanhGiaPhongVan
        {
            Id = Guid.NewGuid().ToString(),
            LichPhongVanId = vm.LichPhongVanId,
            NhanVienDanhGiaId = nhanVien.MaNhanVien,
            DiemDanhGia = vm.DiemDanhGia,
            NhanXet = vm.NhanXet,
            DeXuat = vm.DeXuat
        };

        _context.DanhGiaPhongVans.Add(danhGia);

        var lich = await _context.LichPhongVans.FindAsync(vm.LichPhongVanId);
        if (lich != null)
        {
            if (vm.DeXuat == "TiepNhan")
                lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();
            else if (vm.DeXuat == "TuChoi")
                lich.TrangThai = TrangThaiPhongVanEnum.Huy.ToString();
        }


        await _context.SaveChangesAsync();
        return true;
    }



}
