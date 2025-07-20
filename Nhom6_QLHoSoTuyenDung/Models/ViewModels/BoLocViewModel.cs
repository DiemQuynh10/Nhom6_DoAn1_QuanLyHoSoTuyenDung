using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class BoLocViewModel
    {
        public string? Keyword { get; set; }
        public string? TrangThai { get; set; }
        public string? GioiTinh { get; set; }
        public string? ViTriId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }

        public List<SelectListItem>? ViTriList { get; set; }
        public List<SelectListItem>? GioiTinhList { get; set; }

        public string ResetUrl { get; set; } = "/UngViens";
    }

}
