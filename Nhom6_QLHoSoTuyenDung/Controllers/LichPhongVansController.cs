using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class LichPhongVansController : Controller
    {
        private readonly AppDbContext _context;

        public LichPhongVansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LichPhongVans/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichPhongVan = await _context.LichPhongVans
                .Include(l => l.PhongPhongVan)
                .Include(l => l.UngVien)
                .Include(l => l.ViTriTuyenDung)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lichPhongVan == null)
            {
                return NotFound();
            }

            return View(lichPhongVan);
        }

        public async Task<IActionResult> ByUngVien(string id)
        {
            var lich = await _context.LichPhongVans
                .Include(l => l.PhongPhongVan)
                .FirstOrDefaultAsync(l => l.UngVienId == id);

            if (lich == null)
            {
                return Content("<p class='text-muted'>Chưa có lịch phỏng vấn.</p><a class='btn btn-primary mt-2' href='/LichPhongVans/Create?ungVienId=" + id + "'>Tạo lịch ngay</a>", "text/html");
            }

            string diaDiem = lich.PhongPhongVan?.DiaDiem ?? "Chưa rõ";

            string html = $@"
                <p><strong>Thời gian:</strong> {lich.ThoiGian:dd/MM/yyyy HH:mm}</p>
                <p><strong>Địa điểm:</strong> {diaDiem}</p>
                <p><strong>Ghi chú:</strong> {lich.GhiChu}</p>";

            return Content(html, "text/html");
        }

        // GET: LichPhongVans/Create
        public IActionResult Create(string ungVienId)
        {
            // Lấy danh sách phòng
            var phongList = _context.PhongPhongVans
                .Select(p => new { p.Id, Ten = p.TenPhong + " - " + p.DiaDiem }).ToList();

            var uvList = _context.UngViens
                .Select(uv => new { uv.MaUngVien, uv.HoTen }).ToList();

            var viTriList = _context.ViTriTuyenDungs
                .Select(v => new { v.MaViTri, v.TenViTri }).ToList();

            string viTriMacDinh = null;

            if (!string.IsNullOrEmpty(ungVienId))
            {
                var ungVien = _context.UngViens.FirstOrDefault(u => u.MaUngVien == ungVienId);
                if (ungVien != null)
                {
                    viTriMacDinh = ungVien.ViTriUngTuyenId;
                }
            }

            ViewBag.PhongPhongVanList = new SelectList(phongList, "Id", "Ten");
            ViewBag.UngVienList = new SelectList(uvList, "MaUngVien", "HoTen", ungVienId);
            ViewBag.ViTriList = new SelectList(viTriList, "MaViTri", "TenViTri", viTriMacDinh);

            return View();
        }


        // POST: LichPhongVans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LichPhongVan lich)
        {
            if (ModelState.IsValid)
            {
                lich.Id = Guid.NewGuid().ToString();
                _context.Add(lich);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã tạo lịch phỏng vấn thành công!";
                return RedirectToAction("Index", "UngViens");
            }
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage); // Debug ra Console
            }

            TempData["Error"] = "Tạo lịch thất bại. Vui lòng kiểm tra lại.";
            await LoadDropdownsAsync(lich.UngVienId);
            return View(lich);
        }
        private async Task LoadDropdownsAsync(string ungVienId = null)
        {
            var phongList = await _context.PhongPhongVans
                .Select(p => new
                {
                    p.Id,
                    Display = p.TenPhong + " - " + p.DiaDiem
                }).ToListAsync();
            ViewBag.PhongPhongVanList = new SelectList(phongList, "Id", "Display");

            var uvList = await _context.UngViens.ToListAsync();
            ViewBag.UngVienList = new SelectList(uvList, "MaUngVien", "HoTen", ungVienId);

            var viTriList = await _context.ViTriTuyenDungs.ToListAsync();
            ViewBag.ViTriList = new SelectList(viTriList, "MaViTri", "TenViTri");
        }

        private bool LichPhongVanExists(string id)
        {
            return _context.LichPhongVans.Any(e => e.Id == id);
        }
    }
}
