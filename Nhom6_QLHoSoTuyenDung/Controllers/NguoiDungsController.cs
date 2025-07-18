// PHẦN XỬ LÝ ĐĂNG NHẬP - TRÍCH XUẤT TỪ NguoiDungsController

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nhom6_QLHoSoTuyenDung.Models.Enums;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System.Security.Claims;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    [AllowAnonymous]
    public partial class NguoiDungsController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;

        public NguoiDungsController(ITaiKhoanService taiKhoanService)
        {
            _taiKhoanService = taiKhoanService;
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetString("VaiTro") != null)
                return RedirectToAction("Index", "Home");
            return View(new DangNhapVM());
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapVM model)
        {
            if (!ModelState.IsValid) return View(model);

            int soLanSai = HttpContext.Session.GetInt32("SoLanSai") ?? 0;
            if (soLanSai >= 5)
            {
                ModelState.AddModelError("", "Bạn đã nhập sai quá nhiều lần. Vui lòng thử lại sau.");
                return View(model);
            }

            var user = await _taiKhoanService.DangNhapAsync(model, HttpContext);
            if (user == null)
            {
                HttpContext.Session.SetInt32("SoLanSai", soLanSai + 1);
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Lưu thông tin vào session nếu cần (không bắt buộc nữa nếu dùng Claims)
            HttpContext.Session.SetString("VaiTro", user.VaiTro);
            HttpContext.Session.SetString("TenDangNhap", user.TenDangNhap);
            HttpContext.Session.SetString("HoTen", user.HoTen ?? "");

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, user.TenDangNhap),
    new Claim(ClaimTypes.Role, user.VaiTro),
    new Claim("HoTen", user.HoTen ?? "")
};

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Đăng nhập bằng scheme "Cookies"
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal);

            // Chuyển hướng tùy theo vai trò
            if (user.VaiTro == RoleNames.Admin || user.VaiTro == RoleNames.HR)
            {
                return RedirectToAction("Index", "UngViens");
            }
            else if (user.VaiTro == RoleNames.Interviewer)
            {
                return RedirectToAction("Index", "InterviewerDashboard");
            }
            else
            {
                return RedirectToAction("Index", "NguoiDungs");
            }

        }
        public IActionResult AccessDenied()
        {
            return View(); // Tạo view AccessDenied.cshtml trong Views/NguoiDungs
        }
        public IActionResult Logout()
        {
            _taiKhoanService.DangXuat(HttpContext);
            return RedirectToAction("DangNhap");
        }

        [HttpGet]
        public IActionResult QuenMatKhau() => View();

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var ma = await _taiKhoanService.GuiMaXacNhanAsync(model.TenDangNhap, model.Email, HttpContext);
            if (ma == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản phù hợp.";
                return View(model);
            }

            TempData["Success"] = "Đã gửi mã xác nhận đến email.";
            return View("XacNhanMa", new XacNhanMaVM { TenDangNhap = model.TenDangNhap });
        }

        [HttpGet]
        public IActionResult XacNhanMa(string tenDangNhap)
        {
            return View("XacNhanMa", new XacNhanMaVM { TenDangNhap = tenDangNhap });
        }

        [HttpPost]
        public IActionResult XacNhanMa(XacNhanMaVM model)
        {
            if (!_taiKhoanService.KiemTraMaXacNhan(HttpContext, model.MaXacNhan))
            {
                TempData["Error"] = "Mã xác nhận không hợp lệ hoặc đã hết hạn.";
                return View(model);
            }
            return RedirectToAction("DatLaiMatKhau", new { tenDangNhap = model.TenDangNhap });
        }

        [HttpGet]
        public IActionResult DatLaiMatKhau(string tenDangNhap)
        {
            return View(new DatLaiMatKhauVM { TenDangNhap = tenDangNhap });
        }

        [HttpPost]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauVM model)
        {
            if (!ModelState.IsValid) return View(model);

            bool ok = await _taiKhoanService.DatLaiMatKhauAsync(model.TenDangNhap, model.MatKhauMoi);
            if (!ok)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return View(model);
            }

            TempData["Success"] = "Đặt lại mật khẩu thành công. Vui lòng đăng nhập lại.";
            return RedirectToAction("DangNhap");
        }
    }
}