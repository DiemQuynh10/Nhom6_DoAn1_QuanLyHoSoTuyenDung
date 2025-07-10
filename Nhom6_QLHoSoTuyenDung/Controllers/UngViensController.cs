using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class UngViensController : Controller
    {
        private readonly IUngVienService _ungVienService;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UngViensController(IUngVienService ungVienService, AppDbContext context, IWebHostEnvironment env)
        {
            _ungVienService = ungVienService;
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

        public async Task<IActionResult> Index(UngVienFilterVM filter, int page = 1, int pageSize = 10)
        {
            if (HttpContext.Session.GetString("VaiTro") == "interviewer")
                return RedirectToAction("Index", "LichPhongVan");

            await LoadDropdownsAsync();

            var allUngViens = await _ungVienService.GetAllAsync(filter);
            var stats = await _ungVienService.GetDashboardStatsAsync(allUngViens);

            ViewBag.TongUngVien = stats["TongUngVien"];
            ViewBag.MoiTuanNay = stats["MoiTuanNay"];
            ViewBag.DaPhongVan = stats["DaPhongVan"];
            ViewBag.DaTuyen = stats["DaTuyen"];
            ViewBag.TyLeChuyenDoi = stats["TyLeChuyenDoi"];
            ViewBag.NguonLabels = stats["NguonLabels"];
            ViewBag.NguonValues = stats["NguonValues"];
            ViewBag.TrangThaiLabels = stats["TrangThaiLabels"];
            ViewBag.TrangThaiValues = stats["TrangThaiValues"];
            ViewBag.ViTriLabels = stats["ViTriLabels"];
            ViewBag.ViTriValues = stats["ViTriValues"];

            var filterViewModel = new FilterViewModel
            {
                Keyword = filter.Keyword,
                TrangThai = filter.TrangThai,
                GioiTinh = filter.GioiTinh,
                ViTriId = filter.ViTriId,
                FromDate = filter.FromDate?.ToString("yyyy-MM-dd"),
                ToDate = filter.ToDate?.ToString("yyyy-MM-dd"),
                ViTriList = ((SelectList)ViewBag.ViTriList).ToList(),
                GioiTinhList = ((SelectList)ViewBag.GioiTinhList).ToList(),
                ResetUrl = "/UngViens"
            };
            ViewBag.FilterViewModel = filterViewModel;

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

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var ungVien = await _ungVienService.GetByIdAsync(id);
            if (ungVien == null) return NotFound();
            return PartialView("_UngVienDetailsPartial", ungVien);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UngVien model, IFormFile CvFile)
        {
            if (!ModelState.IsValid || CvFile == null)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin và chọn CV.";
                return RedirectToAction("Index");
            }

            var result = await _ungVienService.AddAsync(model, CvFile, _env);
            if (result == -1)
            {
                TempData["ErrorMessage"] = "Ứng viên đã tồn tại.";
                return RedirectToAction("Index");
            }

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
                var count = await _ungVienService.ImportFromExcelAsync(excelFile);
                TempData["SuccessMessage"] = $"✅ Đã nhập {count} ứng viên.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Lỗi khi xử lý file: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
