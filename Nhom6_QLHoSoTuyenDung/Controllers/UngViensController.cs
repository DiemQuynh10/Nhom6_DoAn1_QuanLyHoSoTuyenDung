using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models;
using Spire.Doc;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class UngViensController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UngViensController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private async Task LoadDropdownsAsync()
        {
            ViewBag.ViTriList = new SelectList(await _context.ViTriTuyenDungs.ToListAsync(), "MaViTri", "TenViTri");
            ViewBag.GioiTinhList = new SelectList(
    Enum.GetValues(typeof(GioiTinhEnum))
        .Cast<GioiTinhEnum>()
        .Select(gt => new
        {
            Value = gt,
            Text = gt.GetDisplayName()
        }),
    "Value", "Text");
        }
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var ungVien = await _context.UngViens
                .Include(x => x.ViTriUngTuyen)
                .FirstOrDefaultAsync(x => x.MaUngVien == id);

            if (ungVien == null)
                return NotFound();

            return PartialView("_UngVienDetailsPartial", ungVien);
        }
        private IQueryable<UngVien> ApplyFilter(IQueryable<UngVien> query, UngVienFilterVM filter)
        {
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
            return query;
        }

        public async Task<IActionResult> Index(UngVienFilterVM filter, int page = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("VaiTro") == "interviewer")
            {
                return RedirectToAction("Index", "LichPhongVan");
            }
            await LoadDropdownsAsync();
            var query = _context.UngViens.Include(x => x.ViTriUngTuyen).AsQueryable();
            query = ApplyFilter(query, filter);
            var allUngViens = await query.ToListAsync();

            ViewBag.TongUngVien = allUngViens.Count;
            ViewBag.MoiTuanNay = allUngViens.Count(x => x.NgayNop != null && x.NgayNop.Value >= DateTime.Now.AddDays(-7));
            ViewBag.DaPhongVan = allUngViens.Count(x => x.TrangThai != null && x.TrangThai.Contains("Phỏng vấn"));
            ViewBag.DaTuyen = allUngViens.Count(x => x.TrangThai != null && x.TrangThai.Contains("Đã tuyển"));
            int daTuyen = ViewBag.DaTuyen;
            ViewBag.TyLeChuyenDoi = allUngViens.Count == 0 ? 0 : Math.Round((double)daTuyen * 100 / allUngViens.Count, 2);

            ViewBag.NguonLabels = allUngViens.Where(x => !string.IsNullOrEmpty(x.NguonUngTuyen)).GroupBy(x => x.NguonUngTuyen).Select(g => g.Key).ToList();
            ViewBag.NguonValues = allUngViens.Where(x => !string.IsNullOrEmpty(x.NguonUngTuyen)).GroupBy(x => x.NguonUngTuyen).Select(g => g.Count()).ToList();
            ViewBag.TrangThaiLabels = allUngViens.Where(x => !string.IsNullOrEmpty(x.TrangThai)).GroupBy(x => x.TrangThai).Select(g => g.Key).ToList();
            ViewBag.TrangThaiValues = allUngViens.Where(x => !string.IsNullOrEmpty(x.TrangThai)).GroupBy(x => x.TrangThai).Select(g => g.Count()).ToList();
            ViewBag.ViTriLabels = allUngViens.Where(x => x.ViTriUngTuyen != null).GroupBy(x => x.ViTriUngTuyen.TenViTri).Select(g => g.Key).ToList();
            ViewBag.ViTriValues = allUngViens.Where(x => x.ViTriUngTuyen != null).GroupBy(x => x.ViTriUngTuyen.TenViTri).Select(g => g.Count()).ToList();

            var totalItems = allUngViens.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedUngViens = allUngViens.OrderByDescending(x => x.NgayNop).Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.LichPhongVanMap = await _context.LichPhongVans.GroupBy(l => l.UngVienId).ToDictionaryAsync(g => g.Key, g => g.First());

            return View(pagedUngViens);
        }
        [HttpPost]
        public async Task<IActionResult> Create(UngVien model, IFormFile CvFile)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();

                var danhSach = await _context.UngViens.Include(x => x.ViTriUngTuyen).ToListAsync();

                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin hợp lệ.";
                return View("Index", danhSach); // không redirect!
            }
            if (string.IsNullOrEmpty(model.TrangThai))
                model.TrangThai = "Mới";

            // Kiểm tra trùng email (có thể thêm các điều kiện khác nếu cần)
            bool isDuplicate = _context.UngViens.Any(u =>
                u.Email == model.Email &&
                u.HoTen == model.HoTen &&
                u.ViTriUngTuyenId == model.ViTriUngTuyenId
            );
            if (isDuplicate)
            {
                TempData["ErrorMessage"] = "Ứng viên đã tồn tại.";
                return RedirectToAction("Index");
            }

            // Xử lý upload file và lưu model
            string fileName = Guid.NewGuid() + Path.GetExtension(CvFile.FileName);
            string path = Path.Combine(_env.WebRootPath, "cv", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await CvFile.CopyToAsync(stream);
            }

            model.MaUngVien = Guid.NewGuid().ToString();
            model.LinkCV = "/cv/" + fileName;
            model.NgayNop = DateTime.Now;

            _context.UngViens.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thêm ứng viên thành công!";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file Excel hợp lệ.";
                return RedirectToAction("Index");
            }

            try
            {
                var viTriDict = _context.ViTriTuyenDungs
                    .ToList();

                int importedCount = 0;

                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1); // sheet đầu tiên
                        var rows = worksheet.RowsUsed().Skip(1); // bỏ tiêu đề

                        foreach (var row in rows)
                        {
                            try
                            {
                                var viTriTen = row.Cell(6).GetString().Trim();
                                var viTri = viTriDict
                                    .FirstOrDefault(v => v.TenViTri.Trim().Equals(viTriTen, StringComparison.OrdinalIgnoreCase));
                                if (viTri == null)
                                {
                                    Console.WriteLine($"❌ Không tìm thấy vị trí: {viTriTen}");
                                    continue;
                                }

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
                            catch (Exception exRow)
                            {
                                Console.WriteLine($"❌ Lỗi đọc dòng Excel: {exRow.Message}");
                                continue;
                            }
                        }

                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var fullMessage = ex.InnerException?.Message ?? ex.Message;
                            TempData["ErrorMessage"] = $"❌ Lỗi khi lưu vào DB: {fullMessage}";
                        }
                    }
                }

                TempData["SuccessMessage"] = $"✅ Đã nhập {importedCount} ứng viên.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Lỗi tổng quát: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


        private GioiTinhEnum ParseGioiTinh(string value)
        {
            return EnumExtensions.GetEnumFromDisplayName<GioiTinhEnum>(value) ?? GioiTinhEnum.Khac;
        }

        public class UngVienFilterVM
        {
            public string? Keyword { get; set; }
            public string? GioiTinh { get; set; }
            public string? ViTriId { get; set; }
            public string? TrangThai { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
        }
    }
}
