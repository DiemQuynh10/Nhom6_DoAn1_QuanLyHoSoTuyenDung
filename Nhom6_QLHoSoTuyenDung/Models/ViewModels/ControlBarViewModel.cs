namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels
{
    public class ControlBarViewModel
    {
        public string Title { get; set; } = "";
        public string Icon { get; set; } = "bi bi-folder"; // ví dụ: bi bi-people-fill
        public string? CreateTargetId { get; set; } // ví dụ: #addUngVienModal
        public string? ImportTargetId { get; set; } // ví dụ: #importExcelModal
        public bool ShowFilterButton { get; set; } = true;
    }
}
