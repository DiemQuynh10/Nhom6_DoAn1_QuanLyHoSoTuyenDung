using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
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
            KinhNghiem = lich.UngVien?.KinhNghiem,
            UngVienId = lich.UngVien?.MaUngVien
        };
    }

    public async Task<bool> LuuAsync(DanhGiaPhongVanVM vm, string nguoiDungId)
    {
        var nhanVien = await _context.NhanViens
            .FirstOrDefaultAsync(n => n.MaNhanVien == nguoiDungId);
        if (nhanVien == null) return false;

        var danhGia = await _context.DanhGiaPhongVans
            .FirstOrDefaultAsync(d => d.LichPhongVanId == vm.LichPhongVanId &&
                                      d.NhanVienDanhGiaId == nhanVien.MaNhanVien);

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

        // ✅ Cập nhật điểm số và nhận xét
        danhGia.DiemDanhGia = (int)vm.DiemDanhGia;
        danhGia.NhanXet = vm.NhanXet;

        if (vm.DeXuat.HasValue)
        {
            var deXuat = vm.DeXuat.Value;
            danhGia.DeXuat = deXuat.ToString();

            // ✅ Cập nhật trạng thái ứng viên tương ứng
            var lich = await _context.LichPhongVans
                .Include(l => l.UngVien)
                .FirstOrDefaultAsync(l => l.Id == vm.LichPhongVanId);

            if (lich?.UngVien != null)
            {
                switch (deXuat)
                {
                    case DeXuatEnum.TiepNhan:
                        lich.UngVien.TrangThai = TrangThaiUngVienEnum.DaTuyen.ToString();
                        break;

                    case DeXuatEnum.TuChoi:
                        lich.UngVien.TrangThai = TrangThaiUngVienEnum.TuChoi.ToString();
                        break;

                    case DeXuatEnum.CanPhongVanLan2:
                        lich.UngVien.TrangThai = TrangThaiUngVienEnum.CanPhongVanLan2.ToString();
                        break;
                }

                // ✅ Đánh dấu lịch phỏng vấn đã hoàn thành
                if (lich.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString())
                {
                    lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> LuuChiTietAsync(DanhGiaChiTietVM vm, string nguoiDungId)
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
                NhanVienDanhGiaId = nhanVien.MaNhanVien,
                NgayDanhGia = DateTime.Now
            };
            _context.DanhGiaPhongVans.Add(danhGia);
        }

        // Cập nhật thông tin đánh giá chi tiết
        danhGia.KyNangChuyenMon = vm.KyNangChuyenMon;
        danhGia.GiaoTiep = vm.GiaoTiep;
        danhGia.GiaiQuyetVanDe = vm.GiaiQuyetVanDe;
        danhGia.ThaiDoLamViec = vm.ThaiDoLamViec;
        danhGia.TinhThanHocHoi = vm.TinhThanHocHoi;
        danhGia.DiemDanhGia = vm.DiemDanhGia;
        danhGia.NhanXet = vm.NhanXet;
        if (Enum.TryParse<DeXuatEnum>(vm.DeXuat, out var deXuatEnum))
        {
            danhGia.DeXuat = deXuatEnum.ToString(); // ✅ Lưu đúng như "TiepNhan", "TuChoi"
        }
        else
        {
            danhGia.DeXuat = null; // hoặc xử lý nếu sai định dạng
        }


        // Lấy lịch và cập nhật trạng thái ứng viên
        var lich = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .FirstOrDefaultAsync(l => l.Id == vm.LichPhongVanId);
        // 2. Cập nhật trạng thái
        if (lich?.UngVien != null && danhGia.DeXuat != null)
        {
            switch (deXuatEnum)
            {
                case DeXuatEnum.TiepNhan:
                    lich.UngVien.TrangThai = TrangThaiUngVienEnum.DaTuyen.ToString(); break;
                case DeXuatEnum.TuChoi:
                    lich.UngVien.TrangThai = TrangThaiUngVienEnum.TuChoi.ToString(); break;
                case DeXuatEnum.CanPhongVanLan2:
                    lich.UngVien.TrangThai = TrangThaiUngVienEnum.CanPhongVanLan2.ToString(); break;
            }

            // Lịch phỏng vấn đã hoàn thành
            if (lich.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString())
                lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();
        }


        await _context.SaveChangesAsync();
        return true;
    }


}
