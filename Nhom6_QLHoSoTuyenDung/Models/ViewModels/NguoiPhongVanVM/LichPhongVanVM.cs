using System.ComponentModel.DataAnnotations;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class LichPhongVanVM
    {
        public string Id { get; set; } = "";
        public DateTime? ThoiGian { get; set; }
        public int ThoiLuong { get; set; }
        public string NhanNhan { get; set; } = "";

        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string ViTri { get; set; } = "";
        public string KinhNghiem { get; set; } = "";
        public string TenPhong { get; set; } = "";
        public string DiaDiem { get; set; }
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = string.Empty;

        public string UngVienId { get; set; } = "";   // ✅ ID của ứng viên
        public string? LinkCV { get; set; }           // ✅ Đường dẫn CV của ứng viên (nếu có)
        public bool IsTreHen =>
     ThoiGian.HasValue &&
     ThoiGian.Value <= DateTime.Now &&
     !string.IsNullOrEmpty(TrangThai) &&
     TrangThai != TrangThaiPhongVanEnum.HoanThanh.ToString() &&
     TrangThai != TrangThaiPhongVanEnum.Huy.ToString();

        public bool HienThiNutBatDau => ThoiGian.HasValue && ThoiGian.Value <= DateTime.Now.AddHours(1) && ThoiGian.Value > DateTime.Now;


    }


}
