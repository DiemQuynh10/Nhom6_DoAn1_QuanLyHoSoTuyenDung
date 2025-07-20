using System.ComponentModel.DataAnnotations;

public enum TrangThaiPhongVanEnum
{
    [Display(Name = "Đã lên lịch")]
    DaLenLich,

    [Display(Name = "Hoàn thành")]
    HoanThanh,

    [Display(Name = "Hủy")]
    Huy
}
