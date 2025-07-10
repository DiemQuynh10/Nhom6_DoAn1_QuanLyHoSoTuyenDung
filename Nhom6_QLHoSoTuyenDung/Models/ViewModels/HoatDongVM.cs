namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class HoatDongVM
    {
        public string Loai { get; set; } = "";         // create, upload, schedule, complete
        public string TieuDe { get; set; } = "";       // Tiêu đề hiển thị như "Ứng viên mới"
        public string NoiDung { get; set; } = "";      // Mô tả hoạt động
        public string Icon { get; set; } = "";         // Icon bootstrap hoặc mã Unicode
        public string NguoiThucHien { get; set; } = ""; // VD: Nguyễn Thị Lan
        public DateTime ThoiGian { get; set; }         // Thời gian tạo
    }
}
