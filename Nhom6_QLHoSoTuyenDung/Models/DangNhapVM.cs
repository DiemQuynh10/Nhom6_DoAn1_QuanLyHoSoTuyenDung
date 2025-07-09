using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class DangNhapVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }
}
