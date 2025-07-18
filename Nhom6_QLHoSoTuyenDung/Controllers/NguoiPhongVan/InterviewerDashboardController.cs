using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Services.Implementations;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nhom6_QLHoSoTuyenDung.Controllers.NguoiPhongVan
{
    [Authorize(Roles = $"{RoleNames.Interviewer}")]
    public class InterviewerDashboardController : Controller
    {
        private readonly INguoiPhongVanService _phongVanService;

        public InterviewerDashboardController(INguoiPhongVanService phongVanService)
        {
            _phongVanService = phongVanService;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name ?? "";
            var vm = await _phongVanService.GetDashboardAsync(username);
            return View("DashboardInterviewer", vm);
        }

        public async Task<IActionResult> LichPhongVan()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("DangNhap", "NguoiDungs");

            var dashboard = await _phongVanService.GetDashboardAsync(username);

            var lichSapToi = dashboard.LichPhongVanSapToi
                .OrderBy(l => l.ThoiGian)
                .ToList();

            var lichGanNhat = lichSapToi.FirstOrDefault();
            var lichConLai = lichSapToi.Skip(1).ToList();

            ViewBag.LichGanNhat = lichGanNhat;
            return View(lichConLai);
        }

        [HttpGet]
        public async Task<IActionResult> TrangThaiPhongVanChart()
        {
            var username = User.Identity?.Name ?? "";
            var dashboard = await _phongVanService.GetDashboardAsync(username);
            var danhSach = dashboard.LichPhongVanSapToi;

            var daXacNhan = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.DaLenLich.ToString());
            var choXacNhan = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString());
            var hoanThanh = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.HoanThanh.ToString());
            var daHuy = danhSach.Count(l => l.TrangThai == TrangThaiPhongVanEnum.Huy.ToString());

            return Json(new
            {
                labels = new[] { "Đã xác nhận", "Chờ xác nhận", "Hoàn thành", "Đã hủy" },
                values = new[] { daXacNhan, choXacNhan, hoanThanh, daHuy }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ThanhTichCuaToi()
        {
            var username = User.Identity?.Name ?? "";
            var dashboard = await _phongVanService.GetDashboardAsync(username);
            var tong = dashboard.TongSoPhongVan;
            var tyLe = dashboard.TyLeThanhCong;
            var soThanhCong = dashboard.SoDaHoanThanhHomNay;
            var soKhongPhuHop = tong - soThanhCong;

            return Json(new
            {
                Ten = username,
                ViTri = "Người phỏng vấn",
                TongPV = tong,
                ThanhCong = soThanhCong,
                KhongPhuHop = soKhongPhuHop,
                TyLe = tyLe,
                XepHang = 2,
                TongNguoi = 8
            });
        }

        [HttpGet]
        public async Task<IActionResult> HoatDongGanDay()
        {
            var username = User.Identity?.Name ?? "";
            var dashboard = await _phongVanService.GetDashboardAsync(username);
            return Json(dashboard.HoatDongGanDay);
        }
    }
}
