﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;

namespace Nhom6_QLHoSoTuyenDung.Controllers
{
    public class NguoiDungsController : Controller
    {
        private readonly ITaiKhoanService _svc;
        private const int MAX_FAILED = 5;
        private const int OTP_EXPIRE_SEC = 30;

        public NguoiDungsController(ITaiKhoanService svc)
            => _svc = svc;

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult DangNhap() => View(new DangNhapVM());


        [HttpPost]
        public async Task<IActionResult> DangNhap(DangNhapVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            // ✅ Nếu có dấu @ thì kiểm tra định dạng email
            if (vm.TenDangNhap.Contains("@"))
            {
                var emailValid = System.Text.RegularExpressions.Regex.IsMatch(vm.TenDangNhap,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

                if (!emailValid)
                {
                    ModelState.AddModelError("TenDangNhap", "Email không hợp lệ.");
                    return View(vm);
                }
            }

            var fails = HttpContext.Session.GetInt32("SoLanSai") ?? 0;
            if (fails >= MAX_FAILED)
            {
                ModelState.AddModelError("", $"Bạn đã thử quá {MAX_FAILED} lần. Vui lòng thử lại sau.");
                return View(vm);
            }

            var user = await _svc.DangNhapAsync(vm, HttpContext);
            if (user == null)
            {
                HttpContext.Session.SetInt32("SoLanSai", fails + 1);
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(vm);
            }

            // ✅ Reset đếm sai
            HttpContext.Session.SetInt32("SoLanSai", 0);

            // ✅ Đăng nhập bằng Claims (an toàn với cả user/pass & Gmail)
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.NhanVienId),
    new Claim(ClaimTypes.Name, user.TenDangNhap),
    new Claim(ClaimTypes.Role, user.VaiTro),
    new Claim("HoTen", user.HoTen ?? "Người dùng")
};

            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);

            // ✅ Nếu vẫn cần Session, có thể giữ lại
            HttpContext.Session.SetString("IDNguoiDung", user.NhanVienId);
            HttpContext.Session.SetString("HoTen", user.HoTen ?? "Người dùng");
            HttpContext.Session.SetString("VaiTro", user.VaiTro);


            HttpContext.Session.SetInt32("SoLanSai", 0);
            var role = user.VaiTro.Trim().ToLowerInvariant();

            var dest = role switch
            {
                "admin" => ("Home", "Index"),
                "hr" => ("Home", "Index"),
                "interviewer" => ("InterviewerDashboard", "Index"),
                _ => ("NguoiDungs", "DangNhap")
            };

            return RedirectToAction(dest.Item2, dest.Item1);

        }

        // --- Quên mật khẩu (hiển thị form) ---
        [HttpGet]
        public IActionResult QuenMatKhau() => View(new QuenMatKhauVM());

        // --- Hiển thị form nhập email để gửi mã OTP ---
        [HttpPost]
        public async Task<JsonResult> QuenMatKhauAjax([FromBody] QuenMatKhauVM vm)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, error = "Vui lòng nhập đầy đủ thông tin." });

            var errorMessage = await _svc.GuiMaXacNhanAsync(vm.TenDangNhap.Trim(), vm.Email.Trim().ToLowerInvariant(), HttpContext);

            if (!string.IsNullOrEmpty(errorMessage))
                return Json(new { success = false, error = errorMessage }); // Gửi lỗi ra giao diện

            return Json(new { success = true }); // ✅ gửi thành công, không trả mã
        }


        // --- Hiển thị form nhập mã OTP, đồng thời tính SecondsLeft (thời gian còn lại trước khi mã hết hạn) ---
        [HttpGet]
        public IActionResult XacNhanMa(string tenDangNhap)
        {
            var timeStr = HttpContext.Session.GetString("ThoiGianMa");
            int rem = OTP_EXPIRE_SEC;
            if (!string.IsNullOrEmpty(timeStr))
            {
                var issued = DateTime.Parse(timeStr).ToUniversalTime();
                var elapsed = (DateTime.UtcNow - issued).TotalSeconds;
                rem = OTP_EXPIRE_SEC - (int)elapsed;
                if (rem < 0) rem = 0;
            }
            ViewBag.SecondsLeft = rem;
            return View(new XacNhanMaVM { TenDangNhap = tenDangNhap });
        }

        // --- Kiểm tra mã OTP người dùng nhập vào ---
        [HttpPost]
        public JsonResult VerifyOtpAjax([FromBody] XacNhanMaVM vm)
        {
            var timeStr = HttpContext.Session.GetString("ThoiGianMa");
            if (string.IsNullOrEmpty(timeStr))
                return Json(new { success = false, error = "Phiên đã hết, vui lòng gửi lại mã." });

            var issued = DateTime.Parse(timeStr).ToUniversalTime();
            var elapsed = (DateTime.UtcNow - issued).TotalSeconds;
            if (elapsed > OTP_EXPIRE_SEC)
                return Json(new { success = false, error = "⛔ Mã đã hết hạn." });

            if (!_svc.KiemTraMaXacNhan(HttpContext, vm.MaXacNhan))
                return Json(new { success = false, error = "Mã sai. Vui lòng thử lại." });

            return Json(new { success = true });
        }

        // --- Gửi lại mã xác nhận mới nếu người dùng yêu cầu---
        [HttpPost]
        public async Task<JsonResult> ResendOtp()
        {
            var key = HttpContext.Session.GetString("Otp_User");
            var email = HttpContext.Session.GetString("Otp_Email");

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(email))
                return Json(new { success = false, error = "Phiên đã hết, vui lòng thử lại Quên mật khẩu." });

            var result = await _svc.GuiMaXacNhanAsync(key, email, HttpContext);
            if (!string.IsNullOrEmpty(result))
                return Json(new { success = false, error = result });

            HttpContext.Session.SetString("ThoiGianMa", DateTime.UtcNow.ToString("O"));
            return Json(new { success = true });
        }

        // --- Đặt lại mật khẩu ---
        [HttpGet]
        public IActionResult DatLaiMatKhau(string tenDangNhap)
            => View(new DatLaiMatKhauVM { TenDangNhap = tenDangNhap });

        [HttpPost]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var ok = await _svc.DatLaiMatKhauAsync(vm.TenDangNhap, vm.MatKhauMoi);
            if (!ok)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return View(vm);
            }

            TempData["Success"] = "Đổi mật khẩu thành công. Mời đăng nhập lại.";
            return RedirectToAction("DangNhap");
        }

        // --- Logout ---
        public async Task<IActionResult> Logout()
        {
            // 1. Đăng xuất khỏi hệ thống (xoá cookie đăng nhập)
            await HttpContext.SignOutAsync();

            // 2. Xoá toàn bộ session
            HttpContext.Session.Clear();

            // 3. Xoá cache
            HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            HttpContext.Response.Headers["Pragma"] = "no-cache";
            HttpContext.Response.Headers["Expires"] = "0";

            return RedirectToAction("DangNhap");
        }


    }
}