using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM
{
    public class DanhGiaChiTietVM
    {
        [Required]
        public string LichPhongVanId { get; set; }

        [Display(Name = "Kỹ năng chuyên môn")]
        [Range(0, 10)]
        public int KyNangChuyenMon { get; set; }

        [Display(Name = "Kỹ năng giao tiếp")]
        [Range(0, 10)]
        public int GiaoTiep { get; set; }

        [Display(Name = "Giải quyết vấn đề")]
        [Range(0, 10)]
        public int GiaiQuyetVanDe { get; set; }

        [Display(Name = "Thái độ – Tinh thần")]
        [Range(0, 10)]
        public int ThaiDo { get; set; }

        [Display(Name = "Khả năng học hỏi")]
        [Range(0, 10)]
        public int HocHoi { get; set; }

        [Display(Name = "Nhận xét tổng quát")]
        public string? NhanXet { get; set; }

        [Display(Name = "Đề xuất")]
        public string? DeXuat { get; set; }

        public double DiemTrungBinh =>
            Math.Round((KyNangChuyenMon + GiaoTiep + GiaiQuyetVanDe + ThaiDo + HocHoi) / 5.0, 2);
    }
}
