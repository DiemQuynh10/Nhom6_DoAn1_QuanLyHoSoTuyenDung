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
            var chuaCoLich = await _lichService.GetUngViensChuaCoLichAsync();
            ViewBag.UngViensChuaCoLich = chuaCoLich;
            return View(dashboard);
        }

        // 2. Trả về popup tạo lịch (giao diện HR)
        public async Task<IActionResult> TaoLichPopup(string ungVienId)
        {
            var vm = await _lichService.GetFormDataAsync(ungVienId);
            if (vm == null)
                return NotFound();

            // ✅ Lọc người dùng có vai trò là Người phỏng vấn
            var nguoiPhongVanIds = await _context.NguoiDungs
                .Where(nd => nd.VaiTro == "Interviewer") // hoặc dùng enum nếu có
                .Select(nd => nd.NhanVienId)
                .ToListAsync();

            // ✅ Lấy danh sách nhân viên tương ứng
            vm.NguoiPhongVanOptions = await _context.NhanViens
                .Where(nv => nguoiPhongVanIds.Contains(nv.MaNhanVien))
                .Select(nv => new SelectListItem
                {
                    Value = nv.MaNhanVien,
                    Text = nv.HoTen + " (" + nv.Email + ")"
                }).ToListAsync();

            return PartialView("_FormTaoLichPhongVan", vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLichFromPopup(TaoLichPhongVanVM vm)
        {
            if (string.IsNullOrEmpty(vm.TrangThai))
                vm.TrangThai = TrangThaiPhongVanEnum.DaLenLich.ToString();

            // Tạo model lịch từ ViewModel
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

            // Gán danh sách người phỏng vấn để kiểm tra
            model.NhanVienThamGiaPVs = vm.NguoiPhongVanIds
                .Select(id => new NhanVienThamGiaPhongVan
                {
                    Id = Guid.NewGuid().ToString(),
                    NhanVienId = id,
                    LichPhongVanId = model.Id,
                    VaiTro = "Phỏng vấn viên"
                }).ToList();

            // 🧠 GỌI VÀ KIỂM TRA QUA SERVICE
            var (success, message) = await _lichService.CreateLichAsync(model);
            if (!success)
            {
                return Json(new { success = false, message });
            }

            return Json(new { success = true, message });
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
