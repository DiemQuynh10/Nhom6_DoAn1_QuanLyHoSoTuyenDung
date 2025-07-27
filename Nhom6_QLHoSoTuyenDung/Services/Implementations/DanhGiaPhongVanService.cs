using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

public class DanhGiaPhongVanService : IDanhGiaPhongVanService
{
    private readonly AppDbContext _context;
    private readonly ITaiKhoanService _taiKhoanService;

    public DanhGiaPhongVanService(AppDbContext context, ITaiKhoanService taiKhoanService)
    {
        _context = context;
        _taiKhoanService = taiKhoanService;
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
        var nhanVien = await _context.NhanViens.FirstOrDefaultAsync(n => n.MaNhanVien == nguoiDungId);
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
                NhanVienDanhGiaId = nhanVien.MaNhanVien,
                NgayDanhGia = DateTime.Now
            };
            _context.DanhGiaPhongVans.Add(danhGia);
        }

        danhGia.DiemDanhGia = (int)vm.DiemDanhGia;
        danhGia.NhanXet = vm.NhanXet;
        danhGia.DeXuat = vm.DeXuat?.ToString();

        await CapNhatTrangThaiUngVienAsync(vm.LichPhongVanId, vm.DeXuat?.ToString());
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LuuChiTietAsync(DanhGiaChiTietVM vm, string nguoiDungId)
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
                NhanVienDanhGiaId = nhanVien.MaNhanVien,
                NgayDanhGia = DateTime.Now
            };
            _context.DanhGiaPhongVans.Add(danhGia);
        }

        danhGia.KyNangChuyenMon = vm.KyNangChuyenMon;
        danhGia.GiaoTiep = vm.GiaoTiep;
        danhGia.GiaiQuyetVanDe = vm.GiaiQuyetVanDe;
        danhGia.ThaiDoLamViec = vm.ThaiDoLamViec;
        danhGia.TinhThanHocHoi = vm.TinhThanHocHoi;
        danhGia.DiemDanhGia = vm.DiemDanhGia;
        danhGia.NhanXet = vm.NhanXet;
        danhGia.DeXuat = vm.DeXuat;

        await CapNhatTrangThaiUngVienAsync(vm.LichPhongVanId, vm.DeXuat);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task CapNhatTrangThaiUngVienAsync(string lichId, string? deXuatStr)
    {
        if (string.IsNullOrEmpty(deXuatStr)) return;

        var lich = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .Include(l => l.ViTriTuyenDung)
            .FirstOrDefaultAsync(l => l.Id == lichId);

        if (lich?.UngVien == null || string.IsNullOrEmpty(lich.UngVien.Email)) return;
        if (!Enum.TryParse<DeXuatEnum>(deXuatStr, out var deXuat)) return;

        // Cập nhật trạng thái ứng viên
        lich.UngVien.TrangThai = deXuat switch
        {
            DeXuatEnum.TiepNhan => TrangThaiUngVienEnum.DaTuyen.ToString(),
            DeXuatEnum.TuChoi => TrangThaiUngVienEnum.TuChoi.ToString(),
            DeXuatEnum.CanPhongVanLan2 => TrangThaiUngVienEnum.CanPhongVanLan2.ToString(),
            _ => lich.UngVien.TrangThai
        };

        // Cập nhật trạng thái lịch
        if (lich.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString())
            lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();

        // Gửi email
        var subject = GetEmailSubject(deXuat);
        var body = GetEmailBody(deXuat, lich.UngVien.HoTen, lich.ViTriTuyenDung?.TenViTri);
        await _taiKhoanService.SendEmailAsync(lich.UngVien.Email, subject, body);
    }

    private string GetEmailSubject(DeXuatEnum deXuat) => deXuat switch
    {
        DeXuatEnum.TiepNhan => "🎉 Thông báo trúng tuyển – Chào mừng bạn đến với công ty!",
        DeXuatEnum.TuChoi => "🙁 Kết quả phỏng vấn – Cảm ơn bạn đã tham gia",
        DeXuatEnum.CanPhongVanLan2 => "📢 Mời tham gia phỏng vấn vòng 2",
        _ => "Thông báo kết quả phỏng vấn"
    };

    private string GetEmailBody(DeXuatEnum deXuat, string hoTen, string? tenViTri)
    {
        return deXuat switch
        {
            DeXuatEnum.TiepNhan =>
                $"Thân gửi {hoTen},\n\n" +
                $"Chúc mừng bạn đã được chọn cho vị trí: {tenViTri}.\n" +
                $"Nhân sự sẽ sớm liên hệ để hướng dẫn nhận việc.\n\n" +
                $"Trân trọng,\nPhòng Tuyển dụng",

            DeXuatEnum.TuChoi =>
                $"Thân gửi {hoTen},\n\n" +
                $"Cảm ơn bạn đã tham gia phỏng vấn cho vị trí: {tenViTri}.\n" +
                $"Rất tiếc, bạn chưa đạt yêu cầu ở thời điểm hiện tại.\n" +
                $"Chúc bạn thành công trong tương lai!\n\n" +
                $"Trân trọng,\nPhòng Tuyển dụng",

            DeXuatEnum.CanPhongVanLan2 =>
                $"Thân gửi {hoTen},\n\n" +
                $"Bạn đã được chọn vào vòng phỏng vấn thứ 2 cho vị trí: {tenViTri}.\n" +
                $"Lịch hẹn sẽ được gửi trong thời gian tới.\n\n" +
                $"Trân trọng,\nPhòng Tuyển dụng",

            _ => "Thông báo kết quả phỏng vấn."
        };
    }

    

}
