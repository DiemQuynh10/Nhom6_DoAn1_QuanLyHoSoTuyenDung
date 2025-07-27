using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.Enums
{
    public enum TrangThaiUngVienEnum
    {
        [Display(Name = "Mới")]
        Moi,

        [Display(Name = "Đã phỏng vấn")]
        DaPhongVan,

        [Display(Name = "Cần phỏng vấn lần 2")]
        CanPhongVanLan2,

        [Display(Name = "Đã có lịch vòng 2")]
        DaCoLichVong2,

        [Display(Name = "Đã tuyển")]
        DaTuyen,

        [Display(Name = "Từ chối")]
        TuChoi
    }

}
