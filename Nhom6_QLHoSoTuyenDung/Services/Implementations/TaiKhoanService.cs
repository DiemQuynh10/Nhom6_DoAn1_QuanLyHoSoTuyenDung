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
        private const int OTP_EXPIRE_MIN = 2; // phút

        public TaiKhoanService(AppDbContext db, IOptions<EmailSettings> mailOpt)
        {
            _db = db;
            _mail = mailOpt.Value;
        }

        // Đăng nhập
        public async Task<NguoiDung?> DangNhapAsync(DangNhapVM model, HttpContext http)
        {
            var key = model.TenDangNhap.Trim().ToLowerInvariant();
            var user = await _db.NguoiDungs
                .FirstOrDefaultAsync(u =>
                    (u.TenDangNhap.ToLower() == key || u.Email.ToLower() == key)
                     && u.MatKhau == model.MatKhau);
            if (user == null) return null;

            // Reset số lần sai, lưu session
            http.Session.SetInt32("SoLanSai", 0);
            http.Session.SetString("TenDangNhap", user.TenDangNhap);
            http.Session.SetString("VaiTro", user.VaiTro);
            http.Session.SetString("HoTen", user.HoTen ?? user.TenDangNhap);

            // Tạo cookie với claims
            var roleNormalized = CultureInfo.InvariantCulture.TextInfo
                .ToTitleCase(user.VaiTro.Trim().ToLower());
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.NhanVienId!),
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

        // Đăng xuất
        public void DangXuat(HttpContext http)
        {
            http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).GetAwaiter().GetResult();
            http.Session.Clear();
        }

        // Gửi mã OTP về email
        public async Task<string?> GuiMaXacNhanAsync(string tenDangNhap, string email, HttpContext http)
        {
            var key = tenDangNhap.Trim().ToLowerInvariant();
            var mailLower = email.Trim().ToLowerInvariant();

            var user = await _db.NguoiDungs
                .FirstOrDefaultAsync(u =>
                    u.TenDangNhap.ToLower() == key &&
                    u.Email.ToLower() == mailLower);

            if (user == null)
                return "Không tìm thấy tài khoản với thông tin đã nhập.";

            // Sinh mã OTP ngẫu nhiên gồm 6 chữ số
            var rng = new Random();
            string otp = rng.Next(100000, 999999).ToString();

            // Lưu OTP và các thông tin vào session
            http.Session.SetString("Otp_Ma", otp);
            http.Session.SetString("Otp_User", key);
            http.Session.SetString("Otp_Email", mailLower);
            http.Session.SetString("ThoiGianMa", DateTime.UtcNow.ToString("O"));

            // Tạo nội dung email
            var msg = new MailMessage
            {
                From = new MailAddress(_mail.Mail, _mail.DisplayName),
                Subject = "🔐 Mã xác nhận đặt lại mật khẩu",
                Body = $"Xin chào {user.HoTen ?? "bạn"},\n\n" +
                       $"Mã xác nhận đặt lại mật khẩu của bạn là: {otp}\n" +
                       $"Mã sẽ hết hạn sau {OTP_EXPIRE_MIN} phút.\n\n" +
                       $"Nếu bạn không yêu cầu, hãy bỏ qua email này.",
                IsBodyHtml = false
            };
            msg.To.Add(user.Email!);

            try
            {
                using var client = new SmtpClient(_mail.Host, _mail.Port)
                {
                    Credentials = new NetworkCredential(_mail.Mail, _mail.Password),
                    EnableSsl = true
                };
                await client.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                return $"Lỗi gửi email: {ex.Message}";
            }

            return null; // ✅ Thành công
        }

        // Kiểm tra OTP
        public bool KiemTraMaXacNhan(HttpContext http, string maNhap)
        {
            var stored = http.Session.GetString("Otp_Ma");
            if (string.IsNullOrEmpty(stored)) return false;

            // Kiểm tra thời gian
            var timeStr = http.Session.GetString("ThoiGianMa");
            if (timeStr != null)
            {
                var issued = DateTime.Parse(timeStr).ToUniversalTime();
                if ((DateTime.UtcNow - issued).TotalMinutes > OTP_EXPIRE_MIN)
                    return false;
            }
            return stored == maNhap.Trim();
        }

        // Đặt lại mật khẩu
        public async Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi)
        {
            var key = tenDangNhap.Trim().ToLowerInvariant();
            var user = await _db.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap.ToLower() == key);
            if (user == null) return false;

            user.MatKhau = matKhauMoi; // hoặc hash nếu cần
            await _db.SaveChangesAsync();
            return true;
        }

        // Tìm người dùng (nếu cần)
        public async Task<NguoiDung?> TimNguoiDungAsync(string key)
        {
            key = key.Trim().ToLowerInvariant();
            return await _db.NguoiDungs
                .FirstOrDefaultAsync(u =>
                    u.TenDangNhap.ToLower() == key || u.Email.ToLower() == key);
        }
    }
}
