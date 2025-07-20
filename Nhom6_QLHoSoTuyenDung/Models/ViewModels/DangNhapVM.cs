using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Vui lòng nhập username hoặc email")]
        public string TenDangNhap { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = "";

        public bool GhiNho { get; set; }
    }
}