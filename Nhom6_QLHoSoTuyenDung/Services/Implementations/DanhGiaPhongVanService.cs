using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Implementations;
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

                        await _taiKhoanService.SendEmailAsync(
                            lich.UngVien.Email,
                            "Thông báo trúng tuyển",
                            $"Chúc mừng {lich.UngVien.HoTen}, bạn đã trúng tuyển và sẽ sớm được liên hệ nhận việc."
                        );
                        break;

                    case DeXuatEnum.TuChoi:
                        lich.UngVien.TrangThai = TrangThaiUngVienEnum.TuChoi.ToString();

                        await _taiKhoanService.SendEmailAsync(
                            lich.UngVien.Email,
                            "Kết quả phỏng vấn",
                            $"Cảm ơn bạn {lich.UngVien.HoTen} đã tham gia phỏng vấn. Rất tiếc, kết quả lần này bạn chưa đạt yêu cầu. Hẹn gặp lại bạn trong những cơ hội khác!"
                        );
                        break;

                    case DeXuatEnum.CanPhongVanLan2:
                        lich.UngVien.TrangThai = TrangThaiUngVienEnum.CanPhongVanLan2.ToString();

                        await _taiKhoanService.SendEmailAsync(
                            lich.UngVien.Email,
                            "Phỏng vấn vòng 2",
                            $"Bạn {lich.UngVien.HoTen} đã được chọn vào vòng phỏng vấn thứ 2. Vui lòng truy cập hệ thống để kiểm tra lịch phỏng vấn tiếp theo."
                        );
                        break;
                }

                // ✅ Đánh dấu lịch hoàn thành nếu chưa
                if (lich.TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString())
                    lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();
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
        if (lich?.UngVien != null && !string.IsNullOrEmpty(lich.UngVien.Email))
        {
            switch (deXuatEnum)
            {
                case DeXuatEnum.TiepNhan:
                    await _taiKhoanService.SendEmailAsync(
    lich.UngVien.Email,
    "🎉 Thông báo trúng tuyển – Chào mừng bạn đến với công ty!",
    $"Thân gửi {lich.UngVien.HoTen},\n\n" +
    $"Chúc mừng bạn đã vượt qua các vòng tuyển chọn và chính thức được lựa chọn cho vị trí: {lich.ViTriTuyenDung?.TenViTri} tại công ty chúng tôi.\n\n" +
    $"Bộ phận Nhân sự sẽ liên hệ với bạn trong thời gian sớm nhất để hướng dẫn các bước tiếp theo, bao gồm nhận việc và hoàn thiện hồ sơ.\n\n" +
    $"Chúng tôi rất mong được đồng hành cùng bạn trên chặng đường phát triển sắp tới!\n\n" +
    $"Trân trọng,\nPhòng Tuyển dụng"
);

                    break;

                case DeXuatEnum.TuChoi:
                    await _taiKhoanService.SendEmailAsync(
    lich.UngVien.Email,
    "📢 Mời tham gia phỏng vấn vòng 2",
    $"Thân gửi {lich.UngVien.HoTen},\n\n" +
    $"Cảm ơn bạn đã tham gia vòng phỏng vấn vừa qua cho vị trí: {lich.ViTriTuyenDung?.TenViTri}.\n\n" +
    $"Chúng tôi rất ấn tượng với phần thể hiện của bạn và mong muốn tìm hiểu thêm thông qua một buổi **phỏng vấn vòng 2**.\n\n" +
    $"📅 Lịch phỏng vấn vòng 2 sẽ được gửi đến bạn trong thời gian sớm nhất.\n" +
    $"Vui lòng kiểm tra email hoặc đăng nhập vào hệ thống để cập nhật thông tin.\n\n" +
    $"Chúc bạn chuẩn bị thật tốt và hẹn gặp lại trong buổi phỏng vấn tiếp theo!\n\n" +
    $"Trân trọng,\nPhòng Tuyển dụng"
);
                    break;

                case DeXuatEnum.CanPhongVanLan2:
                    await _taiKhoanService.SendEmailAsync(
    lich.UngVien.Email,
    "🙁 Kết quả phỏng vấn – Cảm ơn bạn đã tham gia",
    $"Thân gửi {lich.UngVien.HoTen},\n\n" +
    $"Chúng tôi rất cảm ơn bạn đã dành thời gian tham gia phỏng vấn cho vị trí: {lich.ViTriTuyenDung?.TenViTri}.\n\n" +
    $"Sau khi cân nhắc kỹ lưỡng, chúng tôi rất tiếc phải thông báo rằng bạn **chưa phù hợp với yêu cầu tuyển dụng ở thời điểm hiện tại**.\n\n" +
    $"Tuy nhiên, hồ sơ của bạn sẽ được lưu lại để xem xét cho các cơ hội phù hợp trong tương lai.\n\n" +
    $"Chúc bạn nhiều thành công trên con đường sự nghiệp phía trước!\n\n" +
    $"Trân trọng,\nPhòng Tuyển dụng"
);
                    break;
            }
        }



        await _context.SaveChangesAsync();
        return true;
    }


}
