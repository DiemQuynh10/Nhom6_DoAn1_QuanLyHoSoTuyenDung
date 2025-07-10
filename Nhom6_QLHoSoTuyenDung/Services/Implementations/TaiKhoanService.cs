using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Nhom6_QLHoSoTuyenDung.Data;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.Helpers;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using Nhom6_QLHoSoTuyenDung.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Nhom6_QLHoSoTuyenDung.Services.Implementations
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly AppDbContext _context;
        private readonly EmailSettings _emailSettings;

        public TaiKhoanService(AppDbContext context, IOptions<EmailSettings> emailOptions)
        {
            _context = context;
            _emailSettings = emailOptions.Value;
        }

        public async Task<NguoiDung?> DangNhapAsync(DangNhapVM model, HttpContext http)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u =>
                u.TenDangNhap == model.TenDangNhap &&
                u.MatKhau == model.MatKhau);

            if (user == null)
                return null;

            http.Session.SetInt32("SoLanSai", 0);
            http.Session.SetString("TenDangNhap", user.TenDangNhap);
            http.Session.SetString("VaiTro", user.VaiTro);
            http.Session.SetString("HoTen", user.HoTen);

            return user;
        }

        public async Task<string?> GuiMaXacNhanAsync(string tenDangNhap, string email, HttpContext http)
        {
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap && u.Email == email);

            if (user == null)
                return null;

            string ma = new Random().Next(100000, 999999).ToString();
            http.Session.SetString("MaXacNhan", ma);
            http.Session.SetString("ThoiGianMa", DateTime.Now.ToString("O"));

            var message = new MailMessage(_emailSettings.Mail, user.Email)
            {
                Subject = "Mã xác nhận khôi phục mật khẩu",
                Body = $"Mã xác nhận của bạn là: {ma}"
            };

            using var smtp = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
            {
                Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
            return ma;
        }

        public bool KiemTraMaXacNhan(HttpContext http, string maNhap)
        {
            var ma = http.Session.GetString("MaXacNhan");
            var thoiGianStr = http.Session.GetString("ThoiGianMa");

            if (string.IsNullOrEmpty(ma) || string.IsNullOrEmpty(thoiGianStr))
                return false;

            DateTime thoiGianTao = DateTime.Parse(thoiGianStr);
            if ((DateTime.Now - thoiGianTao).TotalMinutes > 5)
                return false;

            return maNhap == ma;
        }

        public async Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap);
            if (user == null)
                return false;

            user.MatKhau = matKhauMoi;
            await _context.SaveChangesAsync();
            return true;
        }

        public void DangXuat(HttpContext http)
        {
            http.Session.Clear();
        }

        public async Task<NguoiDung?> TimNguoiDungAsync(string tenDangNhap)
        {
            return await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap);
        }
    }
}
