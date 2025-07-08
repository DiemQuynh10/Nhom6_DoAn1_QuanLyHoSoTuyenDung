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
    public class ViTriTuyenDungsController : Controller
    {
        private readonly AppDbContext _context;

        public ViTriTuyenDungsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? keyword, string? trangThai, string? phongBanId)
        {
            var query = _context.ViTriTuyenDungs
                .Include(v => v.PhongBan)
                .Include(v => v.UngViens)
                .Include(v => v.LichPhongVans)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(v => v.TenViTri.Contains(keyword));

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(v => v.TrangThai == trangThai);

            if (!string.IsNullOrEmpty(phongBanId))
                query = query.Where(v => v.PhongBanId == phongBanId);

            var dsViTri = query.ToList();

            var phanBoTrangThai = ThongKeViTriHelper.DemTheoTrangThai(dsViTri);
            var (thang, soLuongMoi) = ThongKeViTriHelper.DemTheoThang(dsViTri);
            var soLuongHoanThanh = DemSoLuongHoanThanhTheoThang(dsViTri);

            var dsUngVien = _context.UngViens.ToList();
            var quyTrinh = ThongKeViTriHelper.ThongKeQuyTrinhTuyenDung(dsUngVien);

            var vm = new BieuDoViTriVM
            {
                DanhSachViTri = dsViTri,
                PhanBoTrangThai = phanBoTrangThai,
                Thang = thang,
                SoLuongViTriMoi = soLuongMoi,
                SoLuongHoanThanh = soLuongHoanThanh,
                QuyTrinhTuyenDung = quyTrinh
            };

            ViewBag.PhongBans = _context.PhongBans.ToList();
            ViewBag.CurrentKeyword = keyword;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentPhongBanId = phongBanId;

            return View(vm);
        }

        private List<int> DemSoLuongHoanThanhTheoThang(List<ViTriTuyenDung> dsViTri)
        {
            var result = new List<int>();

            for (int i = 5; i >= 0; i--)
            {
                var thang = DateTime.Now.AddMonths(-i);
                int count = dsViTri
                    .Where(v => v.NgayTao.HasValue && v.TrangThai == "Đã đóng")
                    .Count(v =>
                        v.NgayTao.Value.Month == thang.Month &&
                        v.NgayTao.Value.Year == thang.Year);

                result.Add(count);
            }

            return result;
        }


        [HttpPost]
        public IActionResult CreatePopup(ViTriTuyenDung model)
        {
            if (ModelState.IsValid)
            {
                model.MaViTri = Guid.NewGuid().ToString();
                model.NgayTao = DateTime.Now;

                _context.ViTriTuyenDungs.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Đã thêm vị trí thành công!";
            }

            return RedirectToAction("Index");
        }

        private List<QuyTrinhTuyenDungItem> GetThongKeQuyTrinh(List<UngVien> ungViens)
        {
            DateTime dauTuan = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime cuoiTuan = dauTuan.AddDays(7);

            int hoSoMoi = ungViens.Count(u => u.NgayNop >= dauTuan && u.NgayNop < cuoiTuan);
            int dangXemXet = ungViens.Count(u => u.TrangThai == "Đang xem xét");
            int phongVan = ungViens.Count(u => u.TrangThai == "Phỏng vấn");
            int deXuat = ungViens.Count(u => u.TrangThai == "Đề xuất");

            return new List<QuyTrinhTuyenDungItem>
            {
                new() { Ten = "Hồ sơ mới", SoLuong = hoSoMoi, GhiChu = "Tuần này", PhanTramThayDoi = 0 },
                new() { Ten = "Đang xem xét", SoLuong = dangXemXet, GhiChu = "Chờ duyệt", PhanTramThayDoi = 0 },
                new() { Ten = "Phỏng vấn", SoLuong = phongVan, GhiChu = "Đã lên lịch", PhanTramThayDoi = 0 },
                new() { Ten = "Đề xuất", SoLuong = deXuat, GhiChu = "Chờ quyết định", PhanTramThayDoi = 0 },
            };
        }

        // Các action còn lại giữ nguyên không đổi...

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var viTriTuyenDung = await _context.ViTriTuyenDungs.Include(v => v.PhongBan).FirstOrDefaultAsync(m => m.MaViTri == id);
            if (viTriTuyenDung == null) return NotFound();
            return View(viTriTuyenDung);
        }

        public IActionResult Create()
        {
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaViTri,TenViTri,SoLuongCanTuyen,TrangThai,PhongBanId,KyNang,NgayTao")] ViTriTuyenDung viTriTuyenDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viTriTuyenDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTriTuyenDung.PhongBanId);
            return View(viTriTuyenDung);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var viTriTuyenDung = await _context.ViTriTuyenDungs.FindAsync(id);
            if (viTriTuyenDung == null) return NotFound();
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTriTuyenDung.PhongBanId);
            return View(viTriTuyenDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaViTri,TenViTri,SoLuongCanTuyen,TrangThai,PhongBanId,KyNang,NgayTao")] ViTriTuyenDung viTriTuyenDung)
        {
            if (id != viTriTuyenDung.MaViTri) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viTriTuyenDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViTriTuyenDungExists(viTriTuyenDung.MaViTri)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTriTuyenDung.PhongBanId);
            return View(viTriTuyenDung);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var viTriTuyenDung = await _context.ViTriTuyenDungs.Include(v => v.PhongBan).FirstOrDefaultAsync(m => m.MaViTri == id);
            if (viTriTuyenDung == null) return NotFound();
            return View(viTriTuyenDung);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var viTriTuyenDung = await _context.ViTriTuyenDungs.FindAsync(id);
            if (viTriTuyenDung != null)
            {
                _context.ViTriTuyenDungs.Remove(viTriTuyenDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViTriTuyenDungExists(string id)
        {
            return _context.ViTriTuyenDungs.Any(e => e.MaViTri == id);
        }
    }
}
