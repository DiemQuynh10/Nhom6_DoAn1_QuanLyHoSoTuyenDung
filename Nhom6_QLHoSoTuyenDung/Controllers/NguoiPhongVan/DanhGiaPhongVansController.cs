using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM;
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
        [HttpGet]
        public async Task<IActionResult> DanhGia(string id, string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "LichPhongVan";

            var vm = await _service.GetFormAsync(id);
            if (vm == null) return NotFound();

            var lich = await _context.LichPhongVans.FindAsync(id);
            var danhGia = await _context.DanhGiaPhongVans.FirstOrDefaultAsync(d => d.LichPhongVanId == id);

            ViewBag.TrangThai = lich?.TrangThai;
            ViewBag.DaDanhGia = danhGia != null;
            ViewBag.NhanXet = danhGia?.NhanXet;
            ViewBag.DiemTrungBinh = danhGia?.DiemDanhGia;

            // 👉 Thêm phần này để truyền dữ liệu cho popup đánh giá chi tiết:
            ViewBag.KyNangChuyenMon = danhGia?.KyNangChuyenMon;
            ViewBag.GiaoTiep = danhGia?.GiaoTiep;
            ViewBag.GiaiQuyetVanDe = danhGia?.GiaiQuyetVanDe;
            ViewBag.ThaiDoLamViec = danhGia?.ThaiDoLamViec;
            ViewBag.TinhThanHocHoi = danhGia?.TinhThanHocHoi;
            ViewBag.DeXuat = danhGia?.DeXuat;

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGia(DanhGiaPhongVanVM vm)
        {
            var nguoiDungId = HttpContext.Session.GetString("IDNguoiDung");
            if (string.IsNullOrEmpty(nguoiDungId))
                return RedirectToAction("DangNhap", "NguoiDungs");

            if (!ModelState.IsValid)
                return View(vm);

            var result = await _service.LuuAsync(vm, nguoiDungId);
            if (!result)
                return Unauthorized();

            TempData["Success"] = "✅ Đã lưu đánh giá ứng viên.";
            return RedirectToAction(nameof(DanhGia), new { id = vm.LichPhongVanId });
        }

        [HttpGet]
        public async Task<IActionResult> DanhGiaChiTiet(string lichPhongVanId)
        {
            var lich = await _context.LichPhongVans.FindAsync(lichPhongVanId);
            if (lich?.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString())
            {
                TempData["Error"] = "Lịch phỏng vấn đã hoàn tất. Không thể chỉnh sửa đánh giá.";
                return RedirectToAction(nameof(DanhGia), new { id = lichPhongVanId });
            }
            var danhGia = await _context.DanhGiaPhongVans.FirstOrDefaultAsync(d => d.LichPhongVanId == lichPhongVanId);

            var vm = new DanhGiaChiTietVM
            {
                LichPhongVanId = lichPhongVanId ?? "",
                GiaoTiep = (int)(danhGia?.GiaoTiep ?? 0),
                KyNangChuyenMon = (int)(danhGia?.KyNangChuyenMon ?? 0),
                GiaiQuyetVanDe = (int)(danhGia?.GiaiQuyetVanDe ?? 0),
                ThaiDoLamViec = (int)(danhGia?.ThaiDoLamViec ?? 0),
                TinhThanHocHoi = (int)(danhGia?.TinhThanHocHoi ?? 0),
                NhanXet = danhGia?.NhanXet,
                DeXuat = danhGia?.DeXuat
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhGiaChiTiet(DanhGiaChiTietVM vm)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Dữ liệu đánh giá không hợp lệ. Vui lòng kiểm tra lại.");
                return View(vm);
            }

            var nguoiDungId = HttpContext.Session.GetString("IDNguoiDung");
            if (string.IsNullOrEmpty(nguoiDungId))
                return RedirectToAction("DangNhap", "NguoiDungs");

            // Tính điểm trung bình
            vm.DiemDanhGia = (vm.KyNangChuyenMon + vm.GiaoTiep + vm.GiaiQuyetVanDe + vm.ThaiDoLamViec + vm.TinhThanHocHoi) / 5f;

            var result = await _service.LuuChiTietAsync(vm, nguoiDungId);
            if (!result)
                return Unauthorized();

            TempData["Success"] = "✅ Đánh giá chi tiết đã được lưu!";
            return RedirectToAction(nameof(DanhGia), new { id = vm.LichPhongVanId });
        }
    }
}
