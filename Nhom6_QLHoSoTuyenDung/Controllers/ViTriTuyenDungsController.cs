using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index(string? keyword, string? trangThai, string? phongBanId)
        {
            var dsViTri = _viTriService.GetAll(keyword, trangThai, phongBanId);
            var phanBoTrangThai = _viTriService.PhanBoTrangThai(dsViTri);
            var (thang, soLuongMoi) = _viTriService.DemTheoThang(dsViTri);
            var soLuongHoanThanh = _viTriService.DemSoLuongHoanThanhTheoThang(dsViTri);
            var dsUngVien = _context.UngViens.ToList();
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

        public IActionResult Create()
        {
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ViTriTuyenDung model)
        {
            if (ModelState.IsValid)
            {
                _viTriService.Create(model);
                TempData["Success"] = "Đã thêm vị trí thành công!";
                return RedirectToAction("Index");
            }

            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", model.PhongBanId);
            return View(model);
        }

        [HttpPost]
        public IActionResult CreatePopup(ViTriTuyenDung model)
        {
            if (ModelState.IsValid)
            {
                _viTriService.Create(model);
                TempData["Success"] = "Đã thêm vị trí thành công!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(string id)
        {
            var viTri = _viTriService.GetById(id);
            if (viTri == null) return NotFound();
            return View(viTri);
        }

        public IActionResult Edit(string id)
        {
            var viTri = _viTriService.GetById(id);
            if (viTri == null) return NotFound();
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", viTri.PhongBanId);
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
            ViewData["PhongBanId"] = new SelectList(_context.PhongBans, "Id", "Id", model.PhongBanId);
            return View(model);
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
    }
}
