using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ViTriTuyenDungVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.HR}")]
    public class ViTriTuyenDungsController : Controller
    {
        private readonly IViTriTuyenDungService _viTriService;
        private readonly AppDbContext _context;

        public ViTriTuyenDungsController(IViTriTuyenDungService viTriService, AppDbContext context)
        {
            _viTriService = viTriService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? keyword, string? trangThai, string? phongBanId)
        {
            var dsViTri = _viTriService.GetAll(keyword, trangThai, phongBanId);
            var phanBoTrangThai = _viTriService.PhanBoTrangThai(dsViTri);
            var (thang, soLuongMoi) = _viTriService.DemTheoThang(dsViTri);
            var soLuongHoanThanh = _viTriService.DemSoLuongHoanThanhTheoThang(dsViTri);

            var dsUngVien = _context.UngViens.Include(u => u.LichPhongVans).ToList();
            var quyTrinh = _viTriService.ThongKeQuyTrinh(dsUngVien);
            var hoatDongGanDay = _viTriService.LayHoatDongGanDay();

            var vm = new BieuDoViTriTuyenDungVM
            {
                DanhSachViTri = dsViTri,
                PhanBoTrangThai = phanBoTrangThai,
                Thang = thang,
                SoLuongViTriMoi = soLuongMoi,
                SoLuongHoanThanh = soLuongHoanThanh,
                QuyTrinhTuyenDung = quyTrinh,
                HoatDongGanDay = hoatDongGanDay
            };

            ViewBag.PhongBans = _context.PhongBans.ToList();
            ViewBag.CurrentKeyword = keyword;
            ViewBag.CurrentTrangThai = trangThai;
            ViewBag.CurrentPhongBanId = phongBanId;

            return View(vm);
        }

        public IActionResult Create()
        {
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "TenPhong");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ViTriTuyenDung model)
        {
            if (_context.ViTriTuyenDungs.Any(v => v.TenViTri.ToLower() == model.TenViTri.ToLower()))
            {
                ModelState.AddModelError("TenViTri", "Tên vị trí đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                _viTriService.Create(model);
                TempData["Success"] = "Đã thêm vị trí thành công!";
                return RedirectToAction("Index");
            }

            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "TenPhong", model.PhongBanId);
            return View(model);
        }

        [HttpPost]
        public IActionResult CreatePopup(ViTriTuyenDung model)
        {
            if (_context.ViTriTuyenDungs.Any(v => v.TenViTri.ToLower() == model.TenViTri.ToLower()))
            {
                TempData["Error"] = "Tên vị trí đã tồn tại!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _viTriService.Create(model);
                TempData["Success"] = "Đã thêm vị trí thành công!";
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            var viTri = _viTriService.GetById(id);
            if (viTri == null) return NotFound();

            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "TenPhong", viTri.PhongBanId);
            return View(viTri);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, ViTriTuyenDung model)
        {
            if (id != model.MaViTri) return NotFound();

            if (ModelState.IsValid)
            {
                _viTriService.Update(model);
                return RedirectToAction("Index");
            }

            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "TenPhong", model.PhongBanId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditPopup(ViTriTuyenDung model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var viTri = await _context.ViTriTuyenDungs.FindAsync(model.MaViTri);
            if (viTri == null) return NotFound();

            viTri.TenViTri = model.TenViTri;
            viTri.PhongBanId = model.PhongBanId;
            viTri.SoLuongCanTuyen = model.SoLuongCanTuyen;
            viTri.TrangThai = model.TrangThai;
            viTri.KyNang = model.KyNang;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var viTri = _viTriService.GetById(id);
            if (viTri == null) return NotFound();

            return View(viTri);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _viTriService.Delete(id);
            return RedirectToAction("Index");
        }

        public IActionResult HoatDongGanDay()
        {
            var hoatDongGanDay = _viTriService.LayHoatDongGanDay();
            return View(hoatDongGanDay);
        }

        public IActionResult TatCaHoatDong()
        {
            var hoatDong7Ngay = _viTriService.LayHoatDongGanDay();
            return View("HoatDongGanDay", hoatDong7Ngay);
        }

        [HttpPost]
        public async Task<IActionResult> CapNhatTrangThai([FromBody] CapNhatTrangThaiVM model)
        {
            var viTri = await _context.ViTriTuyenDungs.FirstOrDefaultAsync(v => v.MaViTri == model.MaViTri);
            if (viTri == null)
                return Json(new { success = false });

            viTri.TrangThai = model.TrangThai;
            bool isAutoPaused = false;

            if (model.TrangThai == "Đang tuyển")
            {
                var soLuongUngVien = await _context.UngViens
                    .CountAsync(uv => uv.ViTriUngTuyenId == viTri.MaViTri);

                if (soLuongUngVien >= viTri.SoLuongCanTuyen)
                {
                    viTri.TrangThai = "Tạm dừng";
                    isAutoPaused = true;
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, isAutoPaused });
        }

        [HttpGet]
        public async Task<IActionResult> GetViTriById(string id)
        {
            var viTri = await _context.ViTriTuyenDungs
                .Include(v => v.UngViens)
                .FirstOrDefaultAsync(v => v.MaViTri == id);

            if (viTri == null) return NotFound();

            var soLuongTrungTuyen = viTri.UngViens?
                .Count(uv => uv.TrangThai == TrangThaiUngVienEnum.DaTuyen.ToString()) ?? 0;

            return Json(new
            {
                viTri.MaViTri,
                viTri.TenViTri,
                viTri.PhongBanId,
                viTri.SoLuongCanTuyen,
                viTri.TrangThai,
                viTri.KyNang,
                SoLuongTrungTuyen = soLuongTrungTuyen
            });
        }
    }
}
