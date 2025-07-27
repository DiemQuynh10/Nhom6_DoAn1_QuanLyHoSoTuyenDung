using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Enums
{
    public enum DeXuatEnum
    {
        [Display(Name = "Tiếp nhận")]
        TiepNhan,

        [Display(Name = "Từ chối")]
        TuChoi,

        [Display(Name = "Cần phỏng vấn lần 2")]
        CanPhongVanLan2
    }
}
