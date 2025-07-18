using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Services.Implementations;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System.Security.Claims;

namespace Nhom6_QLHoSoTuyenDung.Controllers.NguoiPhongVan
{
    [Authorize(Roles = RoleNames.Interviewer)]
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
                return RedirectToAction("DangNhap", "NguoiDung");

            var dashboard = await _phongVanService.GetDashboardAsync(username);

            // Tách lịch gần nhất và phần còn lại
            var lichSapToi = dashboard.LichPhongVanSapToi
                .OrderBy(l => l.ThoiGian)
                .ToList();

            var lichGanNhat = lichSapToi.FirstOrDefault();
            var lichConLai = lichSapToi.Skip(1).ToList();

            ViewBag.LichGanNhat = lichGanNhat;
            return View(lichConLai);
        }

    }
}
