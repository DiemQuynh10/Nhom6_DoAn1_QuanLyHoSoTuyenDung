using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Enums
{
    public enum DeXuatEnum
    {
        [Display(Name = "Không phù hợp")]
        KhongPhuHop,

        [Display(Name = "Tiếp Nhận")]
        TiepNhan,

        [Display(Name = "Cần xem xét thêm")]
        CanXemXet
    }
}
