using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
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

    public async Task<TaoLichPhongVanVM?> GetFormDataAsync(string? ungVienId)
    {
        var phongList = await _context.PhongPhongVans
            .Select(p => new SelectListItem
            {
                Value = p.Id,
                Text = p.TenPhong + " - " + p.DiaDiem
            }).ToListAsync();

        var model = new TaoLichPhongVanVM
        {
            PhongList = phongList,
            NguoiPhongVanOptions = new List<SelectListItem>() // sẽ gán sau từ controller
        };

        if (!string.IsNullOrEmpty(ungVienId))
        {
            var ungVien = await _context.UngViens
                .Include(u => u.ViTriUngTuyen)
                .FirstOrDefaultAsync(u => u.MaUngVien == ungVienId);

            if (ungVien != null)
            {
                model.UngVienId = ungVien.MaUngVien;
                model.TenUngVien = ungVien.HoTen;
                model.ViTriId = ungVien.ViTriUngTuyenId;
                model.TenViTri = ungVien.ViTriUngTuyen?.TenViTri;
            }
        }

        return model;
    }



    public async Task<(bool, string)> CreateLichAsync(LichPhongVan model)
    {
        var ungVien = await _context.UngViens.FirstOrDefaultAsync(u => u.MaUngVien == model.UngVienId);
        if (ungVien == null)
            return (false, "Ứng viên không tồn tại.");

        model.ViTriId = ungVien.ViTriUngTuyenId;
        model.Id = await GenerateNewMaLichAsync();

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
        model.TrangThai = TrangThaiPhongVanEnum.DaLenLich.ToString();

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

    public async Task<List<DaPhongVanVM>> GetUngViensChuaCoLichVong2Async()
    {
        // B1: Lấy các lịch có đánh giá đề xuất phỏng vấn lần 2
        var lichV1 = await _context.LichPhongVans
            .Include(l => l.UngVien)
            .Include(l => l.ViTriTuyenDung)
            .Include(l => l.DanhGiaPhongVans)
            .Where(l => l.DanhGiaPhongVans.Any(d => d.DeXuat == DeXuatEnum.CanPhongVanLan2.ToString()))
            .ToListAsync();

        var result = new List<DaPhongVanVM>();

        foreach (var lich in lichV1)
        {
            var ungVienId = lich.UngVienId;

            // B2: Kiểm tra xem đã có lịch mới cho vòng 2 chưa
            var daCoLichV2 = await _context.LichPhongVans
                .AnyAsync(l => l.UngVienId == ungVienId
                    && l.Id != lich.Id
                    && l.TrangThai == TrangThaiPhongVanEnum.DaLenLich.ToString());

            if (daCoLichV2)
                continue;

            // B3: Lấy thông tin ứng viên
            result.Add(new DaPhongVanVM
            {
                LichId = lich.Id,
                TenUngVien = lich.UngVien?.HoTen ?? "Không rõ",
                UngVienId = lich.UngVienId,
                Email = lich.UngVien?.Email ?? "",
                ViTri = lich.ViTriTuyenDung?.TenViTri ?? "",
                ThoiGian = lich.ThoiGian ?? DateTime.Now,
                LinkCV = lich.UngVien?.LinkCV,
                DiemTB = lich.DanhGiaPhongVans.FirstOrDefault()?.DiemDanhGia,
                NhanXet = lich.DanhGiaPhongVans.FirstOrDefault()?.NhanXet
            });
        }

        return result;
    }
    private async Task<string> GenerateNewMaLichAsync()
    {
        var lastMa = await _context.LichPhongVans
            .OrderByDescending(l => l.Id)
            .Select(l => l.Id)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(lastMa) || !lastMa.StartsWith("LP"))
            return "LP001";

        var number = int.TryParse(lastMa.Substring(2), out int num) ? num : 0;
        return $"LP{(num + 1):D3}";
    }
}
