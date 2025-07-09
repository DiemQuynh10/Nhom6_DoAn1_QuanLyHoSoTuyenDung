using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class DatLaiMatKhauVM
    {
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(6, ErrorMessage = "Mật khẩu ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; }

        [Required]
        [Compare("MatKhauMoi", ErrorMessage = "Xác nhận mật khẩu không khớp")]
        [DataType(DataType.Password)]
        public string XacNhanMatKhauMoi { get; set; }
    }
}
