using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class XacNhanMaVM
    {
        public string TenDangNhap { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã gồm 6 chữ số")]
        public string MaXacNhan { get; set; } = "";
    }
}
