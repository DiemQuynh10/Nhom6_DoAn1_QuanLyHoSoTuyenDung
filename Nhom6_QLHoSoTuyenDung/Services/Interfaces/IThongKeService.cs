using Nhom6_QLHoSoTuyenDung.Models.ViewModels.ThongKe;

namespace Nhom6_QLHoSoTuyenDung.Services.Interfaces
{
    public interface IThongKeService
    {
        // Thống kê tổng quan (dùng ở trang Index)
        Task<List<BieuDoItemVM>> GetBieuDoNguonUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);
        Task<List<BieuDoItemVM>> GetBieuDoTheoViTriUngTuyenAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);
        Task<List<BieuDoItemVM>> GetBieuDoTheoPhongBanAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);
        Task<List<BieuDoItemVM>> GetXuHuongTheoThangAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);
        Task<List<ViTriThanhCongVM>> GetViTriTuyenThanhCongAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);
        Task<List<BieuDoItemVM>> GetBieuDoDanhGiaUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);

        Task<ThongKeTongHopVM> GetTongQuanAsync(string? tuKhoa, string? loai, DateTime? tuNgay, DateTime? denNgay);
        Task<List<BieuDoItemVM>> GetBieuDoTheoTrangThaiUngVienAsync(string? tuKhoa, DateTime? tuNgay, DateTime? denNgay);

    }
}
