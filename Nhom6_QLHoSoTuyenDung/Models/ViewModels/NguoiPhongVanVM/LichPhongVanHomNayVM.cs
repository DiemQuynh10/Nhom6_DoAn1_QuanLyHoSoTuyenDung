namespace Nhom6_QLHoSoTuyenDung.Models.ViewModels.NguoiPhongVanVM
{
    public class LichPhongVanHomNayVM
    {
        public string Id { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string ViTri { get; set; } = "";
        public string PhongBan { get; set; } = "";
        public DateTime GioBatDau { get; set; }
        public DateTime GioKetThuc { get; set; }
        public string HinhThuc { get; set; } = "";
        public string TrangThai { get; set; } = "";
        public int ThoiLuong => (int)(GioKetThuc - GioBatDau).TotalMinutes;
    }

}
