using Microsoft.AspNetCore.Http;
using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;
using System.Threading.Tasks;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface ITaiKhoanService
    {
        /// <summary>Đăng nhập: tạo cookie & trả về entity hoặc null.</summary>
        Task<NguoiDung?> DangNhapAsync(DangNhapVM model, HttpContext http);

        /// <summary>Xoá cookie + session.</summary>
        void DangXuat(HttpContext http);

        Task<string?> GuiMaXacNhanAsync(string tenDangNhap, string email, HttpContext http);
        bool KiemTraMaXacNhan(HttpContext http, string maNhap);
        Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi);

        Task<NguoiDung?> TimNguoiDungAsync(string key);
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}