using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Data;
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

            var lich = await _context.LichPhongVans.FindAsync(id);
            var danhGia = await _context.DanhGiaPhongVans.FirstOrDefaultAsync(d => d.LichPhongVanId == id);

            ViewBag.TrangThai = lich?.TrangThai;
            ViewBag.DaDanhGia = danhGia != null;
            ViewBag.NhanXet = danhGia?.NhanXet;
            ViewBag.DiemTrungBinh = danhGia?.DiemDanhGia;

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
                return View(vm); // vẫn cần validate form

            var result = await _service.LuuAsync(vm, nguoiDungId);
            if (!result)
                return Unauthorized();

            TempData["Success"] = "✅ Đã lưu đánh giá ứng viên.";
            return RedirectToAction(nameof(DanhGia), new { id = vm.LichPhongVanId });
        }


        [HttpGet]
        public async Task<IActionResult> DanhGiaChiTiet(string lichPhongVanId)
        {
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

            float diemTrungBinh = (vm.KyNangChuyenMon + vm.GiaoTiep + vm.GiaiQuyetVanDe + vm.ThaiDoLamViec + vm.TinhThanHocHoi) / 5f;

            var danhGia = await _context.DanhGiaPhongVans.FirstOrDefaultAsync(d => d.LichPhongVanId == vm.LichPhongVanId);
            if (danhGia != null)
            {
                danhGia.KyNangChuyenMon = vm.KyNangChuyenMon;
                danhGia.GiaoTiep = vm.GiaoTiep;
                danhGia.GiaiQuyetVanDe = vm.GiaiQuyetVanDe;
                danhGia.ThaiDoLamViec = vm.ThaiDoLamViec;
                danhGia.TinhThanHocHoi = vm.TinhThanHocHoi;
                danhGia.NhanXet = vm.NhanXet;
                danhGia.DiemDanhGia = diemTrungBinh;
            }
            else
            {
                danhGia = new DanhGiaPhongVan
                {
                    Id = Guid.NewGuid().ToString(),
                    LichPhongVanId = vm.LichPhongVanId,
                    NhanVienDanhGiaId = nguoiDungId,
                    KyNangChuyenMon = vm.KyNangChuyenMon,
                    GiaoTiep = vm.GiaoTiep,
                    GiaiQuyetVanDe = vm.GiaiQuyetVanDe,
                    ThaiDoLamViec = vm.ThaiDoLamViec,
                    TinhThanHocHoi = vm.TinhThanHocHoi,
                    DiemDanhGia = diemTrungBinh,
                    NhanXet = vm.NhanXet,
                    NgayDanhGia = DateTime.Now
                };
                _context.DanhGiaPhongVans.Add(danhGia);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Đánh giá chi tiết đã được lưu!";
            return RedirectToAction(nameof(DanhGia), new { id = vm.LichPhongVanId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(string id, string deXuat)
        {
            var lich = await _context.LichPhongVans.FindAsync(id);
            if (lich == null) return NotFound();

            if (deXuat == "TiepNhan")
                lich.TrangThai = TrangThaiPhongVanEnum.HoanThanh.ToString();
            else if (deXuat == "TuChoi")
                lich.TrangThai = TrangThaiPhongVanEnum.Huy.ToString();

            await _context.SaveChangesAsync();
            TempData["Success"] = "✅ Cập nhật trạng thái thành công!";
            return RedirectToAction(nameof(DanhGia), new { id });
        }
    }
}
