using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class XacNhanMaVM
    {
        [Required]
        public string TenDangNhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã xác nhận")]
        public string MaXacNhan { get; set; }
    }
}
