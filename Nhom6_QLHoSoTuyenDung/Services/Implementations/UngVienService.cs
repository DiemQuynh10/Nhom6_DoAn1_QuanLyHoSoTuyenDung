using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.UngVien;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using static Nhom6_QLHoSoTuyenDung.Controllers.UngViensController;

namespace Nhom6_QLHoSoTuyenDung.Services.Implementations
{
    public class UngVienService : IUngVienService
    {
        private readonly AppDbContext _context;

        public UngVienService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UngVien>> GetAllAsync(UngVienBoLocDonGianVM filter)
        {
            var query = _context.UngViens.Include(x => x.ViTriUngTuyen).AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
                query = query.Where(x => x.HoTen.Contains(filter.Keyword));
            if (!string.IsNullOrEmpty(filter.GioiTinh))
                query = query.Where(x => x.GioiTinh.ToString() == filter.GioiTinh);
            if (!string.IsNullOrEmpty(filter.ViTriId))
                query = query.Where(x => x.ViTriUngTuyenId == filter.ViTriId);
            if (!string.IsNullOrEmpty(filter.TrangThai))
                query = query.Where(x => x.TrangThai != null && x.TrangThai.Contains(filter.TrangThai));
            if (filter.FromDate.HasValue)
                query = query.Where(x => x.NgayNop >= filter.FromDate);
            if (filter.ToDate.HasValue)
                query = query.Where(x => x.NgayNop <= filter.ToDate);

            return await query.ToListAsync();
        }

        public async Task<UngVien?> GetByIdAsync(string id)
        {
            return await _context.UngViens
                .Include(x => x.ViTriUngTuyen)
                .FirstOrDefaultAsync(x => x.MaUngVien == id);
        }

        public async Task<int> AddAsync(UngVien model, IFormFile CvFile, IWebHostEnvironment env)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.HoTen) || string.IsNullOrWhiteSpace(model.Email)
       || string.IsNullOrWhiteSpace(model.ViTriUngTuyenId) || CvFile == null)
            {
                return 0; // lỗi thiếu dữ liệu
            }
            if (string.IsNullOrEmpty(model.TrangThai))
                model.TrangThai = TrangThaiUngVienEnum.Moi.ToString();

            // Kiểm tra trùng
            bool isDuplicate = _context.UngViens.Any(u =>
                u.Email == model.Email &&
                u.HoTen == model.HoTen &&
                u.ViTriUngTuyenId == model.ViTriUngTuyenId
            );
            if (isDuplicate)
                return -1; // báo lỗi trùng
            
            string folder = Path.Combine(env.WebRootPath, "cv");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Lưu file
            string fileName = Guid.NewGuid() + Path.GetExtension(CvFile.FileName);
            string path = Path.Combine(env.WebRootPath, "cv", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await CvFile.CopyToAsync(stream);
            }

            model.MaUngVien = Guid.NewGuid().ToString();
            model.LinkCV = "/cv/" + fileName;
            model.NgayNop = DateTime.Now;

            _context.UngViens.Add(model);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ImportFromExcelAsync(IFormFile file)
        {
            var viTriDict = _context.ViTriTuyenDungs.ToList();
            int importedCount = 0;

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        try
                        {
                            var viTriTen = row.Cell(6).GetString().Trim();
                            var viTri = viTriDict.FirstOrDefault(v => v.TenViTri.Trim().Equals(viTriTen, StringComparison.OrdinalIgnoreCase));
                            if (viTri == null) continue;

                            var ungVien = new UngVien
                            {
                                MaUngVien = Guid.NewGuid().ToString(),
                                HoTen = row.Cell(1).GetString().Trim(),
                                GioiTinh = EnumExtensions.GetEnumFromDisplayName<GioiTinhEnum>(row.Cell(2).GetString())
                                    .GetValueOrDefault(GioiTinhEnum.Khac),
                                NgaySinh = DateTime.TryParse(row.Cell(3).GetString(), out var ns) ? ns : null,
                                SoDienThoai = row.Cell(4).GetString().Trim(),
                                Email = row.Cell(5).GetString().Trim(),
                                ViTriUngTuyenId = viTri.MaViTri,
                                KinhNghiem = row.Cell(7).GetString().Trim(),
                                ThanhTich = row.Cell(8).GetString().Trim(),
                                MoTa = row.Cell(9).GetString().Trim(),
                                TrangThai = row.Cell(10).GetString().Trim(),
                                NgayNop = DateTime.TryParse(row.Cell(11).GetString(), out var nn) ? nn : null,
                                NguonUngTuyen = row.Cell(12).GetString().Trim()
                            };

                            _context.UngViens.Add(ungVien);
                            importedCount++;
                        }
                        catch { continue; }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return importedCount;
        }

        public async Task<Dictionary<string, object>> GetDashboardStatsAsync(List<UngVien> list)
        {
            var stats = new Dictionary<string, object>();

            // ✅ Tổng số ứng viên
            stats["TongUngVien"] = list.Count;

            // ✅ Ứng viên mới trong tuần (chỉ tính trạng thái Mới)
            stats["MoiTuanNay"] = list.Count(x =>
                !string.IsNullOrEmpty(x.TrangThai) &&
                x.TrangThai.Trim().Equals(TrangThaiUngVienEnum.Moi.ToString(), StringComparison.OrdinalIgnoreCase) &&
                x.NgayNop.HasValue &&
                x.NgayNop.Value >= DateTime.Now.AddDays(-7));

            // ✅ Số lượng đã tuyển
            stats["DaTuyen"] = list.Count(x =>
                !string.IsNullOrEmpty(x.TrangThai) &&
                x.TrangThai.Trim().Equals(TrangThaiUngVienEnum.DaTuyen.ToString(), StringComparison.OrdinalIgnoreCase));

            // ✅ Số lượng từ chối
            stats["TuChoi"] = list.Count(x =>
                !string.IsNullOrEmpty(x.TrangThai) &&
                x.TrangThai.Trim().Equals(TrangThaiUngVienEnum.TuChoi.ToString(), StringComparison.OrdinalIgnoreCase));

            // ✅ Đã phỏng vấn (tính theo bảng lịch phỏng vấn)
            stats["DaPhongVan"] = await _context.LichPhongVans
                .Where(x => x.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString())
                .CountAsync();

            // ✅ Tỷ lệ chuyển đổi
            int tongUngVien = (int)stats["TongUngVien"];
            int daTuyen = (int)stats["DaTuyen"];
            stats["TyLeChuyenDoi"] = tongUngVien == 0 ? 0 : Math.Round((double)daTuyen * 100 / tongUngVien, 2);

            // ✅ Biểu đồ nguồn
            // ✅ Gom nhóm nguồn thành 4 nhóm cố định
            var allNguon = list
                .Where(x => !string.IsNullOrEmpty(x.NguonUngTuyen))
                .GroupBy(x => x.NguonUngTuyen.Trim())
                .Select(g => new { Nguon = g.Key, Count = g.Count() })
                .ToList();

            var nguonStats = new Dictionary<string, int>
{
    { "LinkedIn", 0 },
    { "Website công ty", 0 },
    { "Giới thiệu", 0 },
    { "Khác", 0 }
};

            foreach (var item in allNguon)
            {
                var value = item.Nguon.ToLower();
                if (value.Contains("linkedin"))
                    nguonStats["LinkedIn"] += item.Count;
                else if (value.Contains("website"))
                    nguonStats["Website công ty"] += item.Count;
                else if (value.Contains("giới thiệu") || value.Contains("gioi thieu"))
                    nguonStats["Giới thiệu"] += item.Count;
                else
                    nguonStats["Khác"] += item.Count;
            }

            stats["NguonLabels"] = nguonStats.Keys.ToList();
            stats["NguonValues"] = nguonStats.Values.ToList();


            // ✅ Biểu đồ trạng thái – KHÔNG được dùng trực tiếp ToEnum hoặc lỗi gán sai
            var trangThaiGroups = list
    .Where(x => !string.IsNullOrEmpty(x.TrangThai))
    .Select(x =>
    {
        var value = x.TrangThai.Trim();
        bool success = Enum.TryParse<TrangThaiUngVienEnum>(value, ignoreCase: true, out var enumValue);

        if (!success)
            Console.WriteLine($"❌ Không parse được trạng thái: '{value}'");

        return new { IsValid = success, Enum = enumValue };
    })
    .Where(x => x.IsValid)
    .GroupBy(x => x.Enum)
    .Select(g => new
    {
        Label = g.Key.GetDisplayName(),
        Count = g.Count()
    })
    .ToList();

            stats["TrangThaiLabels"] = trangThaiGroups.Select(x => x.Label).ToList();
            stats["TrangThaiValues"] = trangThaiGroups.Select(x => x.Count).ToList();

            // ✅ Biểu đồ theo vị trí
            stats["ViTriLabels"] = list
                .Where(x => x.ViTriUngTuyen != null)
                .GroupBy(x => x.ViTriUngTuyen.TenViTri.Trim())
                .Select(g => g.Key)
                .ToList();

            stats["ViTriValues"] = list
                .Where(x => x.ViTriUngTuyen != null)
                .GroupBy(x => x.ViTriUngTuyen.TenViTri.Trim())
                .Select(g => g.Count())
                .ToList();

            return stats;
        }

    }
}
