using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models
{
    public class QuenMatKhauVM
    {
        [Required]
        public string TenDangNhap { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
