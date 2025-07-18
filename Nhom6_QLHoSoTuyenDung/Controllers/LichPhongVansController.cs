using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using Nhom6_QLHoSoTuyenDung.Data;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [Authorize(Roles = $"{RoleNames.Admin},{RoleNames.HR}")]
    public class LichPhongVansController : Controller
    {
        private readonly ILichPhongVanService _lichService;
        private readonly AppDbContext _context;

        public LichPhongVansController(ILichPhongVanService lichService, AppDbContext context)
        {
            _lichService = lichService;
            _context = context;
        }

        // 1. Dashboard lịch phỏng vấn
        public async Task<IActionResult> Index()
        {
            var dashboard = await _lichService.GetDashboardAsync();
            return View(dashboard);
        }

        // 2. Trả về popup tạo lịch (giao diện HR)
        public async Task<IActionResult> TaoLichPopup(string ungVienId)
        {
            var vm = await _lichService.GetFormDataAsync(ungVienId);
            if (vm == null)
                return NotFound();

            // ✅ Lấy danh sách người phỏng vấn từ bảng trung gian (đã từng tham gia phỏng vấn)
            var nguoiPhongVanIds = await _context.NhanVienThamGiaPhongVans
                .Select(x => x.NhanVienId)
                .Distinct()
                .ToListAsync();

            // ✅ Lấy danh sách nhân viên dùng Id (khóa chính) để hiển thị trong dropdown
            vm.NguoiPhongVanOptions = await _context.NhanViens
                .Where(nv => nguoiPhongVanIds.Contains(nv.MaNhanVien)) // Có thể bỏ nếu muốn hiện tất cả
                .Select(nv => new SelectListItem
                {
                    Value = nv.MaNhanVien, // ✅ dùng GUID (id), không dùng MaNhanVien nữa
                    Text = nv.HoTen + " (" + nv.Email + ")"
                }).ToListAsync();

            return PartialView("_FormTaoLichPhongVan", vm);
        }

        // 3. Xử lý lưu lịch phỏng vấn
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateLichFromPopup(TaoLichPhongVanVM vm)
        {
            if (string.IsNullOrEmpty(vm.TrangThai))
                vm.TrangThai = "Đã lên lịch";

            // ✅ B1: Tạo và lưu lịch phỏng vấn trước (LichPhongVan)
            var model = new LichPhongVan
            {
                Id = Guid.NewGuid().ToString(),
                UngVienId = vm.UngVienId,
                ViTriId = vm.ViTriId,
                PhongPhongVanId = vm.PhongPhongVanId,
                ThoiGian = vm.ThoiGian,
                TrangThai = vm.TrangThai,
                GhiChu = vm.GhiChu
            };

            _context.LichPhongVans.Add(model);
            await _context.SaveChangesAsync(); // ⚠️ PHẢI lưu trước để tránh lỗi FK

            // ✅ B2: Lưu bảng người phỏng vấn sau khi lịch đã được lưu thành công
            if (vm.NguoiPhongVanIds != null && vm.NguoiPhongVanIds.Any())
            {
                foreach (var nvId in vm.NguoiPhongVanIds)
                {
                    _context.NhanVienThamGiaPhongVans.Add(new NhanVienThamGiaPhongVan
                    {
                        Id = Guid.NewGuid().ToString(),
                        LichPhongVanId = model.Id,
                        NhanVienId = nvId, // ✅ phải là ID (GUID) thật của bảng NhanViens
                        VaiTro = "Phỏng vấn viên"
                    });
                }

                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, message = "Tạo lịch phỏng vấn thành công!" });
        }


        // 4. Xem chi tiết lịch phỏng vấn của ứng viên
        public async Task<IActionResult> ByUngVien(string id)
        {
            var lich = await _lichService.GetLichByUngVienIdAsync(id);
            if (lich == null)
                return Content("<p class='text-muted'>Chưa có lịch phỏng vấn cho ứng viên này.</p>", "text/html");

            var html = $@"
                <div>
                    <p><strong>Ứng viên:</strong> {lich.UngVien?.HoTen}</p>
                    <p><strong>Thời gian:</strong> {lich.ThoiGian:dd/MM/yyyy HH:mm}</p>
                    <p><strong>Phòng:</strong> {lich.PhongPhongVan?.TenPhong} - {lich.PhongPhongVan?.DiaDiem}</p>
                    <p><strong>Trạng thái:</strong> {lich.TrangThai}</p>
                    <p><strong>Ghi chú:</strong> {lich.GhiChu}</p>
                </div>";

            return Content(html, "text/html");
        }
    }
}
