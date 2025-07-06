// ================== Controller: UngViensController.cs ==================
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
            ViewBag.GioiTinhList = new SelectList(Enum.GetValues(typeof(UngVien.GioiTinhEnum)).Cast<UngVien.GioiTinhEnum>().Select(gt => new {
                Value = gt,
                Text = gt.GetType().GetMember(gt.ToString())[0]
                    .GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                    .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                    .FirstOrDefault()?.Name ?? gt.ToString()
            }), "Value", "Text");
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