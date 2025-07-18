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
            if (string.IsNullOrEmpty(model.TrangThai))
                model.TrangThai = "Mới";

            // Kiểm tra trùng
            bool isDuplicate = _context.UngViens.Any(u =>
                u.Email == model.Email &&
                u.HoTen == model.HoTen &&
                u.ViTriUngTuyenId == model.ViTriUngTuyenId
            );
            if (isDuplicate)
                return -1; // báo lỗi trùng

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

        public Task<Dictionary<string, object>> GetDashboardStatsAsync(List<UngVien> list)
        {
            var stats = new Dictionary<string, object>
            {
                ["TongUngVien"] = list.Count,
                ["MoiTuanNay"] = list.Count(x => x.NgayNop != null && x.NgayNop.Value >= DateTime.Now.AddDays(-7)),
                ["DaPhongVan"] = list.Count(x => x.TrangThai != null && x.TrangThai.Contains("Phỏng vấn")),
                ["DaTuyen"] = list.Count(x => x.TrangThai != null && x.TrangThai.Contains("Đã tuyển"))
            };

            int daTuyen = (int)stats["DaTuyen"];
            stats["TyLeChuyenDoi"] = list.Count == 0 ? 0 : Math.Round((double)daTuyen * 100 / list.Count, 2);

            stats["NguonLabels"] = list.Where(x => !string.IsNullOrEmpty(x.NguonUngTuyen)).GroupBy(x => x.NguonUngTuyen).Select(g => g.Key).ToList();
            stats["NguonValues"] = list.Where(x => !string.IsNullOrEmpty(x.NguonUngTuyen)).GroupBy(x => x.NguonUngTuyen).Select(g => g.Count()).ToList();
            stats["TrangThaiLabels"] = list.Where(x => !string.IsNullOrEmpty(x.TrangThai)).GroupBy(x => x.TrangThai).Select(g => g.Key).ToList();
            stats["TrangThaiValues"] = list.Where(x => !string.IsNullOrEmpty(x.TrangThai)).GroupBy(x => x.TrangThai).Select(g => g.Count()).ToList();
            stats["ViTriLabels"] = list.Where(x => x.ViTriUngTuyen != null).GroupBy(x => x.ViTriUngTuyen.TenViTri).Select(g => g.Key).ToList();
            stats["ViTriValues"] = list.Where(x => x.ViTriUngTuyen != null).GroupBy(x => x.ViTriUngTuyen.TenViTri).Select(g => g.Count()).ToList();

            return Task.FromResult(stats);
        }
    }
}
