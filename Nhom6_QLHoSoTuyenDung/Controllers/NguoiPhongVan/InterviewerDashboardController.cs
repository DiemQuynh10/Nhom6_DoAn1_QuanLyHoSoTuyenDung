using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nhom6_QLHoSoTuyenDung.Controllers.NguoiPhongVan
{
    [Authorize(Roles = RoleNames.Interviewer)]
    public class InterviewerDashboardController : Controller
    {
        private readonly INguoiPhongVanService _phongVanService;

        public InterviewerDashboardController(INguoiPhongVanService phongVanService)
            => _phongVanService = phongVanService;

        /*──────────────────── DASHBOARD (mới) ───────────────────*/
        // GET: /InterviewerDashboard/DashboardInterviewer
        [HttpGet]
        public async Task<IActionResult> DashboardInterviewer()
        {
            var username = User.Identity?.Name ?? string.Empty;
            var vm = await _phongVanService.GetDashboardAsync(username);
            return View(vm);   // Views/InterviewerDashboard/DashboardInterviewer.cshtml
        }

        /*──────────────────── GIỮ LẠI LINK CŨ /InterviewerDashboard ───────────────────*/
        // GET: /InterviewerDashboard
        [HttpGet]
        public async Task<IActionResult> Index()
            => await DashboardInterviewer();   // chuyển tiếp nội bộ, không duplicate code

        /*──────────────────── LỊCH PHỎNG VẤN ───────────────────*/
        [HttpGet]
        public async Task<IActionResult> LichPhongVan()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("DangNhap", "NguoiDungs");

            var dashboard = await _phongVanService.GetDashboardAsync(username);

            // Tách lịch
            var lichSapToi = dashboard.LichPhongVanSapToi
                                      .OrderBy(l => l.ThoiGian)
                                      .ToList();

            ViewBag.LichGanNhat = lichSapToi.FirstOrDefault();
            var lichConLai = lichSapToi.Skip(1).ToList();

            return View(lichConLai);          // Views/InterviewerDashboard/LichPhongVan.cshtml
        }
    }
}
