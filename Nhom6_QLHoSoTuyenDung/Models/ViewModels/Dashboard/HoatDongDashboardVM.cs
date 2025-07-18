namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.Dashboard
{
    public class HoatDongDashboardVM
    {
        public string HoatDong { get; set; } = "";      // VD: Đã tuyển
        public string UngVien { get; set; } = "";       // VD: Nguyễn Văn A
        public string ViTri { get; set; } = "";         // VD: Nhân viên kinh doanh

        public string NguoiThucHien { get; set; } = ""; // VD: Hệ thống
        public DateTime? ThoiGian { get; set; } // ✅ sửa đúng kiểu                                                // VD: 10:00 hoặc "Hôm qua"

        // Các thuộc tính cũ của bạn
        public string Loai { get; set; } = "";          // create, upload...
        public string TieuDe { get; set; } = "";        // VD: Ứng viên mới
        public string NoiDung { get; set; } = "";       // VD: Ứng viên đã nộp hồ sơ
        public string Icon { get; set; } = "";          // icon bootstrap hoặc Unicode
    }
}
