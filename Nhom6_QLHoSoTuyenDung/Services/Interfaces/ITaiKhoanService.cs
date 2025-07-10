using Nhom6_QLHoSoTuyenDung.Models.Entities;
using Nhom6_QLHoSoTuyenDung.Models.ViewModels;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface ITaiKhoanService
    {
        Task<NguoiDung?> DangNhapAsync(DangNhapVM model, HttpContext http);
        Task<string?> GuiMaXacNhanAsync(string tenDangNhap, string email, HttpContext http);
        bool KiemTraMaXacNhan(HttpContext http, string maNhap);
        Task<bool> DatLaiMatKhauAsync(string tenDangNhap, string matKhauMoi);
        void DangXuat(HttpContext http);
        Task<NguoiDung?> TimNguoiDungAsync(string tenDangNhap);
    }
}
