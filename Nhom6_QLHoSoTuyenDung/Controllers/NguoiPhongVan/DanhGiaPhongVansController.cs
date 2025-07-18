using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Controllers.NguoiPhongVan
{
    [Authorize(Roles = RoleNames.Interviewer)]
    public class DanhGiaPhongVansController : Controller
    {
        private readonly IDanhGiaPhongVanService _service;
        private readonly AppDbContext _context;

        public DanhGiaPhongVansController(IDanhGiaPhongVanService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> DanhGia(string id)
        {
            var vm = await _service.GetFormAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGia(DanhGiaPhongVanVM vm, string deXuat)
        {
            var nguoiDungId = HttpContext.Session.GetInt32("IDNguoiDung");
            if (nguoiDungId == null) return RedirectToAction("DangNhap", "NguoiDungs");

            if (!ModelState.IsValid) return View(vm);

            // Gán đề xuất (TiepNhan hoặc TuChoi)
            vm.DeXuat = deXuat;

            var result = await _service.LuuAsync(vm, nguoiDungId.Value);
            if (!result) return Unauthorized();

            return RedirectToAction("Index", "NguoiPhongVan");
        }

        [Authorize(Roles = RoleNames.Interviewer)] // 🔥 THÊM DÒNG NÀY
        [HttpPost]
        public async Task<IActionResult> DanhGiaChiTiet(DanhGiaChiTietVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var nguoiDungId = HttpContext.Session.GetInt32("IDNguoiDung");
            if (nguoiDungId == null)
                return RedirectToAction("DangNhap", "NguoiDungs");

            var danhGia = new DanhGiaPhongVan
            {
                Id = Guid.NewGuid().ToString(),
                LichPhongVanId = vm.LichPhongVanId,
                DiemDanhGia = (int)Math.Round(vm.DiemTrungBinh),
                NhanXet = vm.NhanXet,
                DeXuat = vm.DeXuat,
                NhanVienDanhGiaId = nguoiDungId.Value.ToString()
            };

            _context.DanhGiaPhongVans.Add(danhGia);

            var lich = await _context.LichPhongVans.FindAsync(vm.LichPhongVanId);
            if (lich != null)
                lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();

            await _context.SaveChangesAsync();

            TempData["Success"] = "Đánh giá đã được lưu.";
            return RedirectToAction("DanhGia", new { id = vm.LichPhongVanId });
        }


        [HttpGet]
        public IActionResult DanhGiaChiTiet(string id)
        {
            var vm = new DanhGiaChiTietVM { LichPhongVanId = id };
            return View(vm);
        }

    }

}
