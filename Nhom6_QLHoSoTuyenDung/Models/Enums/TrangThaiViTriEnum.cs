using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Enums
{
    public enum TrangThaiViTriEnum
    {
        [Display(Name = "Đang tuyển")]
        DangTuyen,

        [Display(Name = "Tạm dừng")]
        TamDung,

        [Display(Name = "Đã đóng")]
        DaDong
    }

}
