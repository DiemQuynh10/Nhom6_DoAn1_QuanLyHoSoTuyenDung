using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Helpers;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Nhom6_QLHoSoTuyenDung.Services.Implementations
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly AppDbContext _db;
        private readonly EmailSettings _mail;
        private const int OTP_EXPIRE_MIN = 2;

        public TaiKhoanService(AppDbContext db, IOptions<EmailSettings> mailOpt)
        {
            _db = db;
            _mail = mailOpt.Value;
        }

        /*──────────────────── ĐĂNG NHẬP ────────────────────*/
        public async Task<NguoiDung?> DangNhapAsync(DangNhapVM model, HttpContext http)
        {
            var key = model.TenDangNhap.Trim().ToLowerInvariant();

            var user = await _db.NguoiDungs.FirstOrDefaultAsync(u =>
                      (u.TenDangNhap.ToLower() == key || u.Email.ToLower() == key)
                   && u.MatKhau == model.MatKhau);          // TODO: dùng hash nếu cần

            if (user == null) return null;

            /* 1️⃣  Session (giữ nguyên) */
            http.Session.SetInt32("SoLanSai", 0);
            http.Session.SetString("TenDangNhap", user.TenDangNhap);
            http.Session.SetString("VaiTro", user.VaiTro);
            http.Session.SetString("HoTen", user.HoTen ?? user.TenDangNhap);

            /* 2️⃣  CHUẨN HÓA Role → “Interviewer” */
            var roleNormalized = CultureInfo.InvariantCulture.TextInfo
                                 .ToTitleCase(user.VaiTro.Trim().ToLower());

            /* 3️⃣  Cookie Claim */
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.NhanVienId),
                new Claim(ClaimTypes.Name,           user.TenDangNhap),
                new Claim(ClaimTypes.Role,           roleNormalized)
            };

            await http.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties
                {
                    IsPersistent = model.GhiNho,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                });

            return user;
        }

        /*──────────────────── ĐĂNG XUẤT ────────────────────*/
        public void DangXuat(HttpContext http)
        {
            http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)
                .GetAwaiter().GetResult();
            http.Session.Clear();
        }

        /*────── (OTP / Reset mật khẩu giữ nguyên – bỏ qua để ngắn gọn) ─────*/
        #region OTP‑&‑Reset
        public async Task<string?> GuiMaXacNhanAsync(string tenDangNhap, string email, HttpContext http) { /* giữ nguyên */ return null; }
        public bool KiemTraMaXacNhan(HttpContext http, string maNhap) { /* giữ nguyên */ return false; }
        public async Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi) { /* giữ nguyên */ return false; }
        public async Task<NguoiDung?> TimNguoiDungAsync(string key) { /* giữ nguyên */ return null; }
        #endregion
    }
}
