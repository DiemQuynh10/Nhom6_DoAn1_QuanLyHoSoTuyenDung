using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.PhongVanVM
{
    public class TaoLichPhongVanVM
    {
        public string UngVienId { get; set; }
        public string TenUngVien { get; set; }

        public string ViTriId { get; set; }
        public string TenViTri { get; set; }

        public string? PhongPhongVanId { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string? TrangThai { get; set; }

        public string? GhiChu { get; set; }

        public List<SelectListItem> PhongList { get; set; } = new();
        public string? NhanVienPhongVanId { get; set; }
        public List<SelectListItem>? NguoiPhongVanOptions { get; set; }
        public List<string> NguoiPhongVanIds { get; set; } = new();
        public bool IsReschedule { get; set; } = false;


    }
}
