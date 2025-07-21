using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Enums
{
    public enum TrangThaiUngVienEnum
    {
        [Display(Name = "Mới")]
        Moi,

        [Display(Name = "Đã tuyển")]
        DaTuyen,

        [Display(Name = "Từ chối")]
        TuChoi
    }
}
