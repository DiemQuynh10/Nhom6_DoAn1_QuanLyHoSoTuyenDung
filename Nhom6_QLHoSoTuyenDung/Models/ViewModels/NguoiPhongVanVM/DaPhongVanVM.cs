using System;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class DaPhongVanVM
    {
        public string LichId { get; set; } = string.Empty;
        public string? UngVienId { get; set; }


        public string TenUngVien { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string ViTri { get; set; } = string.Empty;

        public DateTime ThoiGian { get; set; }

        public string? LinkCV { get; set; }

        public float? DiemTB { get; set; }

        public string? NhanXet { get; set; }
    }
}
