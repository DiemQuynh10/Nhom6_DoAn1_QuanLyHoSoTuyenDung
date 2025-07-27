using System.Collections.Generic;

namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.ThongKe
{
    public class ThongKeTongHopVM
    {
        public int TongUngVien { get; set; }
        public int SoDaTuyen { get; set; }
        public int SoDangXuLy { get; set; }
        public int SoViTriDangTuyen { get; set; }

        // Giữ nguyên tên cũ để không lỗi view, giá trị = 0 vì không dùng được
        public double ThoiGianTuyenTrungBinhNgay { get; set; }

        // Tùy chọn: dùng nếu cần load biểu đồ sẵn từ controller
        public List<BieuDoItemVM> TheoNguon { get; set; } = new();
        public List<BieuDoItemVM> TheoTrangThai { get; set; } = new();
        public List<BieuDoItemVM> TheoViTri { get; set; } = new();
        public List<BieuDoItemVM> TheoPhongBan { get; set; } = new();
        public List<BieuDoItemVM> DiemDanhGia { get; set; } = new();
        public List<BieuDoItemVM> XuHuongTheoThang { get; set; } = new();
        public List<ViTriThanhCongVM> ViTriThanhCong { get; set; } = new();
        public class UngVienTuyenDungVM
        {
            public string HoTen { get; set; }
            public string Email { get; set; }
            public string TenViTri { get; set; }
            public DateTime NgayNop { get; set; }
        }
        public List<UngVienTuyenDungVM> UngVienDaTuyen { get; set; }
    }
}
